from types import SimpleNamespace

import pytest

from techclass.chat.backends import CopilotChatClient, CopilotChatSession


def event(name, **data):
    return SimpleNamespace(
        type=SimpleNamespace(value=name),
        data=SimpleNamespace(**data),
    )


class FakeSdkSession:
    session_id = "test-session"

    def __init__(self, events):
        self.events = events
        self.handler = None

    def on(self, handler):
        self.handler = handler
        return lambda: None

    async def send_and_wait(self, _prompt):
        for item in self.events:
            self.handler(item)
        return event("assistant.message", content="Final answer")

    async def disconnect(self):
        pass


@pytest.mark.asyncio
async def test_copilot_streams_reasoning_delta_and_completed_event_without_duplicates():
    sdk_session = FakeSdkSession([
        event("assistant.reasoning_delta", delta_content="First", reasoning_id="r1"),
        event("assistant.reasoning", content="First, then verify.", reasoning_id="r1"),
        event("assistant.message_delta", delta_content="Final answer"),
    ])
    session = CopilotChatSession(sdk_session)
    reasoning = []
    content = []

    result = await session.send(
        "question",
        on_reasoning=reasoning.append,
        on_content=content.append,
    )

    assert reasoning == ["First", ", then verify."]
    assert content == ["Final answer"]
    assert result == "Final answer"


class FakeSdkClient:
    def __init__(self):
        self.create_options = None

    async def list_models(self):
        supports = SimpleNamespace(reasoning_effort=True)
        capabilities = SimpleNamespace(supports=supports)
        return [SimpleNamespace(
            id="thinking-model",
            capabilities=capabilities,
            supported_reasoning_efforts=["medium"],
        )]

    async def create_session(self, **options):
        self.create_options = options
        return FakeSdkSession([])


@pytest.mark.asyncio
async def test_copilot_requests_detailed_reasoning_for_thinking_models():
    sdk_client = FakeSdkClient()
    client = CopilotChatClient(sdk_client)

    await client.list_models()
    await client.create_session("thinking-model")

    assert sdk_client.create_options["streaming"] is True
    assert sdk_client.create_options["reasoning_summary"] == "detailed"
