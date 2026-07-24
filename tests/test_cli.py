import sys
from argparse import Namespace

import pytest

from techclass.chat import ChatClient, ChatModelInfo
from techclass.console import cli


def test_main_prints_startup_banner_before_web_or_console_dispatch(monkeypatch, capsys):
    async def fake_async_main(_args):
        pass

    monkeypatch.setattr(cli, "async_main", fake_async_main)
    monkeypatch.setattr(sys, "argv", ["techclass", "--web"])

    cli.main()

    assert capsys.readouterr().out == (
        "╔══════════════════════════════════════════════════════════════╗\n"
        "║           LLM Tool for Extraction of Technical Data          ║\n"
        "╚══════════════════════════════════════════════════════════════╝\n"
        "\n"
        "🔍 Checking prerequisites...\n"
        "\n"
    )


class FakeClient(ChatClient):
    provider_name = "test-provider"

    async def prerequisite_details(self):
        return ["Test runtime connected", "Test authentication valid"]

    async def list_models(self):
        return [ChatModelInfo("test-model")]

    async def create_session(self, model):
        raise AssertionError("Web startup must not create a console session")


@pytest.mark.asyncio
async def test_async_main_prints_each_successful_prerequisite(monkeypatch, tmp_path, capsys):
    for name in cli.REQUIRED_PROMPTS:
        prompt = tmp_path / "prompt" / name
        prompt.parent.mkdir(exist_ok=True)
        prompt.touch()

    client = FakeClient()

    async def fake_connect():
        return client

    async def fake_serve(_workspace):
        pass

    monkeypatch.setattr(cli.CopilotChatClient, "connect", fake_connect)
    monkeypatch.setattr("techclass.web.serve", fake_serve)

    await cli.async_main(Namespace(local=False, web=True, model=None, root=tmp_path))

    output = capsys.readouterr().out
    assert "✓ Python " in output
    assert "✓ 4 prompt templates available" in output
    assert "✓ Web interface dependencies available" in output
    assert "✓ Test runtime connected" in output
    assert "✓ Test authentication valid" in output
    assert "✓ 1 test-provider model(s) available" in output
    assert f"✓ Workspace directories ready at {tmp_path}" in output
