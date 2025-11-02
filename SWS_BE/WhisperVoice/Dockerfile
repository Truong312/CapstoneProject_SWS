# syntax=docker/dockerfile:1
FROM python:3.11-slim

# Install ffmpeg for audio processing and basic deps
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the application
COPY main.py .
COPY test.html .

# Default environment (use CPU by default to avoid missing CUDA/cuDNN in container)
ENV WHISPER_MODEL=small
ENV WHISPER_COMPUTE=
ENV WHISPER_DEVICE=cpu

EXPOSE 8001
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8001"]
