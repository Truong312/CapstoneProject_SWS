# syntax=docker/dockerfile:1
FROM python:3.11-slim

# Cài ffmpeg (cho xử lý audio)
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY app.py .

# Các biến môi trường mặc định
ENV WHISPER_MODEL=tiny-int8
ENV WHISPER_LANG=vi
ENV WHISPER_DEVICE=auto

EXPOSE 8000
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
