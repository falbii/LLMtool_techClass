# TechClass Python Package

This folder contains the Python implementation of TechClass.

From this directory:

```powershell
..\.venv\Scripts\python -m pip install -e ".[dev,web]"
..\.venv\Scripts\python -m techclass
```

The default workspace root is the repository root one level above this folder,
so the Python app continues to use the shared `prompt/`, `01_input/`,
`02_output/`, and `web/wwwroot/` directories.
