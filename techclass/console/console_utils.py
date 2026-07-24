"""Color-coded console output helpers for TechClass Python.

Equivalent to C# ConsoleEx class, providing colored terminal output
with consistent formatting and emojis for better user experience.
"""

from __future__ import annotations

import sys
from enum import Enum
from typing import Callable


class Color:
    """ANSI color codes for terminal output."""
    RESET = "\033[0m"
    RED = "\033[91m"
    GREEN = "\033[92m"
    YELLOW = "\033[93m"
    CYAN = "\033[96m"
    DARK_GRAY = "\033[90m"
    WHITE = "\033[97m"


class LogLevel(Enum):
    """Log levels matching C# ConsoleEx."""
    ERROR = "error"
    SUCCESS = "success"
    WARN = "warn"
    INFO = "info"
    DIM = "dim"
    PLAIN = "plain"


# Subscribers for web UI mirroring (like MessageLogged event in C#)
_message_subscribers: list[Callable[[str, str], None]] = []


def subscribe_to_messages(callback: Callable[[str, str], None]) -> None:
    """Register a callback to receive all console messages.
    
    Args:
        callback: Function(level: str, message: str) -> None
    """
    _message_subscribers.append(callback)


def unsubscribe_from_messages(callback: Callable[[str, str], None]) -> None:
    """Remove a message callback."""
    if callback in _message_subscribers:
        _message_subscribers.remove(callback)


def _emit(level: str, message: str) -> None:
    """Emit message to all subscribers."""
    for callback in _message_subscribers:
        try:
            callback(level, message)
        except Exception:
            pass  # Don't let subscriber errors break the main flow


def _write(color_code: str, level: str, message: str) -> None:
    """Write colored message to console and emit to subscribers."""
    # Only apply colors if stdout is a terminal (not redirected to file/pipe)
    if sys.stdout.isatty():
        print(f"{color_code}{message}{Color.RESET}")
    else:
        print(message)
    _emit(level, message)


def error(message: str) -> None:
    """Output error message in red."""
    _write(Color.RED, LogLevel.ERROR.value, f"❌ {message}")


def success(message: str) -> None:
    """Output success message in green."""
    _write(Color.GREEN, LogLevel.SUCCESS.value, message)


def warn(message: str) -> None:
    """Output warning message in yellow."""
    _write(Color.YELLOW, LogLevel.WARN.value, message)


def info(message: str) -> None:
    """Output info message in cyan."""
    _write(Color.CYAN, LogLevel.INFO.value, message)


def dim(message: str) -> None:
    """Output dim message in dark gray."""
    _write(Color.DARK_GRAY, LogLevel.DIM.value, message)


def plain(message: str) -> None:
    """Output plain message without color (but still emits to subscribers)."""
    print(message)
    _emit(LogLevel.PLAIN.value, message)


# Convenience function for emitting messages without console output
# (used for spinner labels that should appear in web UI but not duplicate in terminal)
def emit(level: str, message: str) -> None:
    """Emit message to subscribers only (no console output)."""
    _emit(level, message)


# Spinner utilities
SPINNER_CHARS = ['|', '/', '-', '\\']


async def run_with_spinner(message: str, action: Callable[[], object]) -> object:
    """Show a spinner while executing an async action, then show success/failure.
    
    Similar to C# Program.RunWithSpinnerAsync.
    """
    import asyncio
    
    print(f"  {message}...", end="", flush=True)
    emit("plain", message)
    
    result = None
    error_occurred = False
    
    async def spinner():
        for char in SPINNER_CHARS * 100:  # Repeat for long operations
            print(f"\r  {message} {char}", end="", flush=True)
            await asyncio.sleep(0.1)
    
    try:
        # Run spinner and action concurrently
        spinner_task = asyncio.create_task(spinner())
        result = await asyncio.wait_for(asyncio.to_thread(action), timeout=300)
        spinner_task.cancel()
        try:
            await spinner_task
        except asyncio.CancelledError:
            pass
        
        # Clear spinner line and show success
        print(f"\r  \033[2K\r{Color.GREEN}✓{Color.RESET}", end="", flush=True)
        print()
        return result
    except Exception as e:
        error_occurred = True
        spinner_task.cancel()
        try:
            await spinner_task
        except asyncio.CancelledError:
            pass
        print(f"\r  \033[2K\r{Color.RED}✗{Color.RESET}", end="", flush=True)
        print()
        raise
