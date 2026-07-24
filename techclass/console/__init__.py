"""Console entry point for TechClass."""

from .console_utils import (
    Color,
    LogLevel,
    dim,
    emit,
    error,
    info,
    plain,
    run_with_spinner,
    subscribe_to_messages,
    success,
    unsubscribe_from_messages,
    warn,
)

__all__ = [
    "Color", "LogLevel", "dim", "emit", "error", "info", 
    "plain", "run_with_spinner", "subscribe_to_messages", 
    "success", "unsubscribe_from_messages", "warn",
]
