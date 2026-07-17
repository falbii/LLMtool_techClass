# Contributing

Thanks for your interest in improving this project.

## Ways to Contribute

- report bugs or unclear behavior with a minimal reproduction
- suggest workflow, documentation, or usability improvements
- contribute code, tests, or reviewer-facing examples
- improve prompt templates or validation logic with clear justification

## Development Setup

1. Install the .NET 10 SDK.
2. Restore dependencies with `dotnet restore`.
3. Choose a backend:
   - GitHub Copilot CLI, or
   - local Ollama with at least one model pulled
4. Run the application with `dotnet run`.

## Tests

Run the deterministic, backend-independent test suite before submitting a pull
request:

```bash
dotnet test TechClass.sln
```

Tests must not require a Copilot subscription, a running Ollama server, or
network access. Add or update tests when changing parsing, normalization,
validation, merging, or output behavior.

## Pull Requests

- keep pull requests focused and small when possible
- explain any behavior changes in the description
- update documentation when commands, paths, or workflow expectations change
- include validation steps you ran locally

## Reporting Issues

When opening an issue, please include:

- operating system
- .NET SDK version
- backend used: Copilot or Ollama
- command sequence that triggered the issue
- sample input characteristics if they matter
- observed behavior and expected behavior

## Support Expectations

This repository is a research software project under active refinement. Best
effort support is provided through the issue tracker. Questions that include a
clear reproduction path are much easier to diagnose and address.
