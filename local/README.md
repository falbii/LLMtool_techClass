# Local models (Ollama)

Run the app against a local [Ollama](https://ollama.com) server instead of GitHub
Copilot — fully offline, no Copilot sign-in required.

## Prerequisites

1. Install Ollama and start the server:
   ```
   ollama serve
   ```
2. Pull at least one model, e.g.:
   ```
   ollama pull llama3.2
   ```

## Usage

Pass `--local` (or `--ollama`) on startup:

```
dotnet run -- --local          # console mode, local models
dotnet run -- --local --web    # web UI, local models
```

In `--local` mode the Copilot prerequisite check is skipped, the model picker
lists your locally installed Ollama models, and chat runs against them.

## Configuration

The server endpoint defaults to `http://localhost:11434`. Override it with the
`OLLAMA_HOST` environment variable (a bare `host:port` or a full URL both work):

```
$env:OLLAMA_HOST = "http://192.168.1.10:11434"
```

## How it fits

[OllamaChatBackend.cs](OllamaChatBackend.cs) implements the same provider-neutral
`IChatClient` / `IChatSession` interfaces (see `../ChatBackend.cs`) as the Copilot
backend, so the rest of the app is unchanged. `/api/chat` is stateless, so each
session keeps its own message history and resends it every turn.
