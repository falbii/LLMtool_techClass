# Chat backends

The Python application holds the provider-neutral chat abstraction and its two
implementations. The rest of the app talks only to the interfaces, so switching
providers changes nothing downstream.

## The abstraction

[techclass/chat/backends.py](../techclass/chat/backends.py) defines the contracts:

- `ChatClient` — connects to a provider, lists available models, and opens sessions.
- `ChatSession` — one conversation: `send` with optional reasoning/content
  delta callbacks for streaming.
- `ChatModelInfo(Id, SupportsReasoning)` — a model as shown in the picker.

## Implementations

- `CopilotChatClient` is the default and uses `github-copilot-sdk`.
- `OllamaChatClient` is selected with
  `--local` / `--ollama`. Talks to a local Ollama server, fully offline.

## Selection

[techclass/console/cli.py](../techclass/console/cli.py) picks the backend at startup: `--local` (or
`--ollama`) constructs `OllamaChatClient`, otherwise `CopilotChatClient` is used.
Everything after that — model picker, sessions, the pipeline commands — is
identical for both.

---

## Local models (Ollama)

Run the app against a local [Ollama](https://ollama.com) server instead of GitHub
Copilot — fully offline, no Copilot sign-in required.

### Prerequisites

1. Install Ollama and start the server:
   ```
   ollama serve
   ```
2. Pull at least one model, e.g.:
   ```
   ollama pull llama3.2
   ```

### Usage

Pass `--local` (or `--ollama`) on startup:

```
techclass --local          # console mode, local models
techclass --local --web    # web UI, local models
```

In `--local` mode no Copilot CLI or sign-in is involved at all; the model picker
lists your locally installed Ollama models, and chat runs against them.
Output files that carry a provider tag (the benchmark results) are marked
`ollama` instead of `copilot`.

### Configuration

The server endpoint defaults to `http://localhost:11434`. Override it with the
`OLLAMA_HOST` environment variable (a bare `host:port` or a full URL both work):

```
$env:OLLAMA_HOST = "http://<YOUR_PRIVATE_IP>:11434"
```

### How it fits

[techclass/chat/backends.py](../techclass/chat/backends.py) implements the same provider-neutral
`ChatClient` / `ChatSession` interfaces for both providers, so the rest of the
app is unchanged. `/api/chat` is
stateless, so each session keeps its own message history and resends it every turn.
