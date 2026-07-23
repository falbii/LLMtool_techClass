"""Chat provider adapters used by the console, web UI, and pipeline."""

from __future__ import annotations

import json
import os
import uuid
from abc import ABC, abstractmethod
from collections.abc import Callable
from typing import Any

import httpx

from ..format_output.models import ChatModelInfo

DeltaHandler = Callable[[str], None]


class ChatSession(ABC):
    """A single model conversation with optional streaming callbacks."""

    session_id: str

    @abstractmethod
    async def send(
        self,
        prompt: str,
        on_reasoning: DeltaHandler | None = None,
        on_content: DeltaHandler | None = None,
    ) -> str: ...

    async def close(self) -> None:
        return None

    async def __aenter__(self) -> "ChatSession":
        return self

    async def __aexit__(self, *_: object) -> None:
        await self.close()


class ChatClient(ABC):
    """Provider-level contract for model discovery and session creation."""

    provider_name: str

    @abstractmethod
    async def list_models(self) -> list[ChatModelInfo]: ...

    @abstractmethod
    async def create_session(self, model: str) -> ChatSession: ...

    async def close(self) -> None:
        return None

    async def __aenter__(self) -> "ChatClient":
        return self

    async def __aexit__(self, *_: object) -> None:
        await self.close()


class CopilotChatClient(ChatClient):
    """GitHub Copilot SDK-backed chat client."""

    provider_name = "copilot"

    def __init__(self, sdk_client: Any) -> None:
        self._client = sdk_client

    @classmethod
    async def connect(cls) -> "CopilotChatClient":
        try:
            from copilot import CopilotClient
        except ImportError as exc:
            raise RuntimeError(
                "GitHub Copilot SDK is not installed. Run: pip install -e ."
            ) from exc

        # SDK 1.0.x honors COPILOT_CLI_PATH itself.
        client = CopilotClient()
        await client.start()
        return cls(client)

    async def list_models(self) -> list[ChatModelInfo]:
        models = await self._client.list_models()
        result: list[ChatModelInfo] = []
        for model in models:
            model_id = _value(model, "id", "")
            capabilities = _value(model, "capabilities", None)
            supports = _nested_value(capabilities, "supports", "reasoning_effort", default=False)
            if not supports:
                supports = bool(_value(model, "supported_reasoning_efforts", []))
            result.append(ChatModelInfo(str(model_id), bool(supports)))
        return result

    async def create_session(self, model: str) -> ChatSession:
        from copilot.session import PermissionHandler

        session = await self._client.create_session(
            on_permission_request=PermissionHandler.approve_all,
            model=model,
            streaming=True,
        )
        return CopilotChatSession(session)

    async def close(self) -> None:
        await self._client.stop()


class CopilotChatSession(ChatSession):
    """Wrap a Copilot SDK session behind the shared chat interface."""

    def __init__(self, session: Any) -> None:
        self._session = session
        self.session_id = str(_value(session, "session_id", ""))

    async def send(
        self,
        prompt: str,
        on_reasoning: DeltaHandler | None = None,
        on_content: DeltaHandler | None = None,
    ) -> str:
        pieces: list[str] = []

        def handle_event(event: Any) -> None:
            # The SDK exposes streamed deltas as typed events; collect only the
            # assistant message content while still forwarding reasoning text.
            event_type = _value(event, "type", "")
            event_name = str(_value(event_type, "value", event_type)).lower()
            data = _value(event, "data", None)
            delta = str(_value(data, "delta_content", "") or "")
            if event_name.endswith("assistant.reasoning_delta") and delta:
                if on_reasoning:
                    on_reasoning(delta)
            elif event_name.endswith("assistant.message_delta") and delta:
                pieces.append(delta)
                if on_content:
                    on_content(delta)

        unsubscribe = self._session.on(handle_event)
        try:
            response = await self._session.send_and_wait(prompt)
        finally:
            unsubscribe()
        if pieces:
            return "".join(pieces)
        data = _value(response, "data", response)
        content = str(_value(data, "content", "") or "")
        if on_content and content:
            on_content(content)
        return content

    async def close(self) -> None:
        await self._session.disconnect()


class OllamaChatClient(ChatClient):
    """HTTP client for a local Ollama server."""

    provider_name = "ollama"

    def __init__(self, http: httpx.AsyncClient) -> None:
        self._http = http

    @classmethod
    async def connect(cls) -> "OllamaChatClient":
        host = os.getenv("OLLAMA_HOST", "http://localhost:11434")
        if not host.startswith(("http://", "https://")):
            host = f"http://{host}"
        http = httpx.AsyncClient(base_url=host.rstrip("/"), timeout=None)
        try:
            response = await http.get("/api/tags")
            response.raise_for_status()
        except Exception as exc:
            await http.aclose()
            raise RuntimeError(f"Could not reach Ollama at {host}: {exc}") from exc
        return cls(http)

    async def list_models(self) -> list[ChatModelInfo]:
        response = await self._http.get("/api/tags")
        response.raise_for_status()
        return [ChatModelInfo(item["name"]) for item in response.json().get("models", [])]

    async def create_session(self, model: str) -> ChatSession:
        return OllamaChatSession(self._http, model)

    async def close(self) -> None:
        await self._http.aclose()


class OllamaChatSession(ChatSession):
    """Stateful Ollama chat session that resends conversation history."""

    def __init__(self, http: httpx.AsyncClient, model: str) -> None:
        self._http = http
        self._model = model
        self._history: list[dict[str, str]] = []
        self.session_id = uuid.uuid4().hex

    async def send(
        self,
        prompt: str,
        on_reasoning: DeltaHandler | None = None,
        on_content: DeltaHandler | None = None,
    ) -> str:
        self._history.append({"role": "user", "content": prompt})
        options = {
            "num_ctx": int(os.getenv("OLLAMA_NUM_CTX", "65536")),
            "temperature": float(os.getenv("OLLAMA_TEMPERATURE", "0")),
            "seed": int(os.getenv("OLLAMA_SEED", "0")),
        }
        pieces: list[str] = []
        payload = {"model": self._model, "messages": self._history, "stream": True, "options": options}
        async with self._http.stream("POST", "/api/chat", json=payload) as response:
            response.raise_for_status()
            async for line in response.aiter_lines():
                if not line:
                    continue
                chunk = json.loads(line)
                if chunk.get("error"):
                    raise RuntimeError(f"Ollama error: {chunk['error']}")
                message = chunk.get("message") or {}
                thinking = message.get("thinking", "")
                content = message.get("content", "")
                if thinking and on_reasoning:
                    on_reasoning(thinking)
                if content:
                    pieces.append(content)
                    if on_content:
                        on_content(content)
                if chunk.get("done"):
                    break
        full = "".join(pieces)
        self._history.append({"role": "assistant", "content": full})
        return full


def _value(value: Any, key: str, default: Any) -> Any:
    """Read a field from either a dictionary or an SDK object."""

    if isinstance(value, dict):
        return value.get(key, default)
    return getattr(value, key, default)


def _nested_value(value: Any, *keys: str, default: Any) -> Any:
    """Read a nested field from mixed dictionary/object SDK responses."""

    current = value
    for key in keys:
        current = _value(current, key, None)
        if current is None:
            return default
    return current
