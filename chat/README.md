# Chat backends

This folder holds the provider-neutral chat abstraction and its two
implementations. The rest of the app talks only to the interfaces, so switching
providers changes nothing downstream.

## The abstraction

[ChatBackend.cs](ChatBackend.cs) defines the contracts:

- `IChatClient` — connects to a provider, lists available models
  (`ListModelsAsync`), and opens sessions.
- `IChatSession` — one conversation: `SendAsync` with optional reasoning/content
  delta callbacks for streaming.
- `ChatModelInfo(Id, SupportsReasoning)` — a model as shown in the picker.

## Implementations

- [CopilotChatBackend.cs](CopilotChatBackend.cs) — `CopilotChatClient`, the
  default. Talks to GitHub Copilot via the `GitHub.Copilot.SDK`.
- [OllamaChatBackend.cs](OllamaChatBackend.cs) — `OllamaChatClient`, selected with
  `--local` / `--ollama`. Talks to a local Ollama server, fully offline.

## Selection

[Program.cs](../Program.cs) picks the backend at startup: `--local` (or
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
dotnet run -- --local          # console mode, local models
dotnet run -- --local --web    # web UI, local models
```

In `--local` mode no Copilot CLI or sign-in is involved at all; the model picker
lists your locally installed Ollama models, and chat runs against them.
Output files that carry a provider tag (the benchmark results) are marked
`ollama` instead of `copilot`.

### Configuration

The server endpoint defaults to `http://localhost:11434`. Override it with the
`OLLAMA_HOST` environment variable (a bare `host:port` or a full URL both work):

```
$env:OLLAMA_HOST = "http://192.168.1.10:11434"
```

### How it fits

[OllamaChatBackend.cs](OllamaChatBackend.cs) implements the same provider-neutral
`IChatClient` / `IChatSession` interfaces (see [ChatBackend.cs](ChatBackend.cs)) as
the Copilot backend, so the rest of the app is unchanged. `/api/chat` is
stateless, so each session keeps its own message history and resends it every turn.
