import os
from pathlib import Path

from fastapi import FastAPI, Request, Response

FILE_PATH = Path(__file__).parent.parent / "Assets/Resources/entertainer.bytes"

app = FastAPI()


@app.get("/")
def root():
    return {"message": "Hello, world!"}


@app.get("/generate")
async def generate_midi(request: Request):
    # data = await request.json()
    # prompt = data.get("prompt", "default melody")
    midi_bytes = await generate_midi_from_prompt("")
    return Response(content=midi_bytes, media_type="audio/midi")


async def generate_midi_from_prompt(prompt: str) -> bytes:
    if os.path.exists(FILE_PATH):
        with open(FILE_PATH, "rb") as f:
            return f.read()
    raise Exception("File not found.")
