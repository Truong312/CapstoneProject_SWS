import os
from fastapi import FastAPI, File, UploadFile, Form, HTTPException, Request
from fastapi.middleware.cors import CORSMiddleware
from faster_whisper import WhisperModel
import tempfile, os as _os, asyncio, logging
from typing import Optional, Any
from contextlib import asynccontextmanager

# Read model configuration from environment so you can easily switch models without code edits
MODEL_NAME = os.getenv("WHISPER_MODEL", "small")  # use 'small' by default for better accuracy than 'tiny'
COMPUTE_TYPE = os.getenv("WHISPER_COMPUTE", "")  # default to empty -> let library pick safe default

@asynccontextmanager
async def lifespan(app: FastAPI):
    global model
    logging.info(f"Loading Whisper model: {MODEL_NAME} (compute={COMPUTE_TYPE})...")
    try:
        # Try to use compute_type if provided
        if COMPUTE_TYPE and COMPUTE_TYPE.lower() not in ("none", "", "null"):
            try:
                model = WhisperModel(MODEL_NAME, device="auto", compute_type=COMPUTE_TYPE)
                logging.info("Model loaded with compute_type=%s", COMPUTE_TYPE)
            except Exception:
                logging.warning("Failed to load model with compute_type=%s; retrying without compute_type", COMPUTE_TYPE)
                logging.exception("compute_type load error")
                model = WhisperModel(MODEL_NAME, device="auto")
                logging.info("Model loaded without compute_type fallback")
        else:
            model = WhisperModel(MODEL_NAME, device="auto")
            logging.info("Model loaded without compute_type")
    except Exception:
        logging.exception("Failed to load Whisper model")
        raise
    try:
        yield
    finally:
        logging.info("Shutting down application")

# Create the FastAPI app with lifespan
app = FastAPI(title="Whisper Speech-to-Text", lifespan=lifespan)

# Development helper: allow all origins to avoid CORS issues when testing from static pages
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Small middleware to log incoming Origin header for easier debugging
@app.middleware("http")
async def log_origin(request: Request, call_next):
    origin = request.headers.get('origin')
    if origin:
        logging.info(f"Incoming request Origin: {origin}")
    else:
        logging.info("Incoming request has no Origin header (likely non-browser or same-origin)")
    return await call_next(request)

# model will be set on startup via lifespan
model: Optional[WhisperModel] = None

@app.get("/")
def home():
    return {"status": "running", "model": MODEL_NAME, "compute": COMPUTE_TYPE}

@app.post("/v1/audio/transcriptions")
async def transcribe(
    file: UploadFile = File(...),
    lang: str = Form("vi"),
    beam_size: int = Form(5),
    temperature: float = Form(0.0),
    task: str = Form("transcribe"),
    word_timestamps: bool = Form(False),
):
    """Transcribe uploaded audio.

    Tunable parameters:
    - lang: language code (use 'auto' to let the model detect)
    - beam_size: increase for better accuracy at cost of speed
    - temperature: lower is more deterministic
    - task: 'transcribe' or 'translate'
    - word_timestamps: whether to include word-level timestamps (slower)
    """
    if model is None:
        raise HTTPException(status_code=503, detail="Model not loaded")

    # write uploaded bytes to a temporary file in binary mode
    with tempfile.NamedTemporaryFile(delete=False, suffix=".wav", mode="wb") as tmp:
        tmp.write(await file.read())
        tmp_path = tmp.name

    text: str = ""
    info: Any = None

    try:
        assert model is not None

        def run_transcribe():
            # language: pass None for auto-detect
            lang_arg = None if lang in ("auto", "", None) else lang
            # call faster-whisper's transcribe with options
            return model.transcribe(
                tmp_path,
                language=lang_arg,
                task=task,
                beam_size=beam_size,
                temperature=temperature,
                word_timestamps=word_timestamps,
            )

        segments, info = await asyncio.to_thread(run_transcribe)
        text = " ".join([seg.text for seg in segments])
    finally:
        try:
            _os.remove(tmp_path)
        except Exception:
            logging.exception("Failed to remove temp file: %s", tmp_path)

    return {
        "text": text,
        "language": lang,
        "duration": getattr(info, 'duration', None),
        "model": MODEL_NAME,
        "compute": COMPUTE_TYPE,
        "params": {"beam_size": beam_size, "temperature": temperature, "task": task, "word_timestamps": word_timestamps},
    }
