# TechClass Python Package

This folder contains the Python implementation of TechClass.

From this directory:

```powershell
..\.venv\Scripts\python -m pip install -e "."
..\.venv\Scripts\python -m techclass
..\.venv\Scripts\python -m techclass --web
```

The default workspace root is the repository root one level above this folder,
so the Python app continues to use the shared `prompt/`, `01_input/`,
`02_output/`, and `web/wwwroot/` directories.
