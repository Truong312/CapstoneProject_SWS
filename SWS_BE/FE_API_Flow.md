Mục tiêu

Tài liệu này mô tả "luồng API" phía FE để:
- Upload file audio tới WhisperController để chuyển voice -> text
- Gửi text tới TextToSqlController (API /api/text-to-sql/query) để chuyển ngôn ngữ tự nhiên -> SQL và thực thi
- Hiển thị kết quả SQL + rows cho người dùng

Base URL (dev)
- http://localhost:8080 (theo thông tin bạn cung cấp)

Tổng quan luồng (FE):
1) FE gửi file audio (multipart/form-data) tới POST /api/whisper/transcribe
   - Response mẫu: { success: true, data: { Text: "...", Language: "vi", Duration: 12.3, ... }, fileName, fileSize }
2) FE lấy giá trị transcription là data.Text (chuỗi) và gửi tới POST /api/text-to-sql/query
   - Body JSON: { "naturalLanguage": "<transcribed text>" }
   - Response mẫu: ResultModel<SqlQueryResultDto> (thông thường chứa Data.Sql và Data.Result)
3) FE hiển thị SQL và kết quả (rows). Nếu có lỗi thì hiển thị thông báo.

Endpoints chi tiết
- POST /api/whisper/transcribe
  - Content-Type: multipart/form-data
  - Field: file (IFormFile)
  - Response 200: { success: true, data: WhisperTranscriptionResponse }
  - Error codes: 400 (bad file), 503 (whisper backend unreachable), 500 (server error)

- POST /api/text-to-sql/query
  - Content-Type: application/json
  - Body: { naturalLanguage: string }
  - Response 200: ResultModel<SqlQueryResultDto> (Data.Sql, Data.Result)
  - Error codes: 400 (missing input), 500 (model/generation/execution error)

Ví dụ FE — Browser (vanilla fetch)

1) Upload file và lấy transcript, tiếp tục gọi TextToSql:

async function uploadAndConvert(file, token) {
  const apiBase = 'http://localhost:8080';

  // 1) upload
  const fd = new FormData();
  fd.append('file', file);

  const whisperResp = await fetch(`${apiBase}/api/whisper/transcribe`, {
    method: 'POST',
    headers: token ? { 'Authorization': `Bearer ${token}` } : undefined,
    body: fd
  });

  if (!whisperResp.ok) {
    const text = await whisperResp.text();
    throw new Error(`Whisper upload failed: ${whisperResp.status} ${text}`);
  }

  const whisperJson = await whisperResp.json();
  const transcript = whisperJson?.data?.Text ?? whisperJson?.data?.text ?? whisperJson?.text;
  if (!transcript) throw new Error('No transcription returned');

  // 2) send to text-to-sql
  const textToSqlResp = await fetch(`${apiBase}/api/text-to-sql/query`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { 'Authorization': `Bearer ${token}` } : {})
    },
    body: JSON.stringify({ naturalLanguage: transcript })
  });

  if (!textToSqlResp.ok) {
    const body = await textToSqlResp.text();
    throw new Error(`TextToSql failed: ${textToSqlResp.status} ${body}`);
  }

  const textToSqlJson = await textToSqlResp.json();
  return { transcript, sqlResult: textToSqlJson };
}

Lưu ý UI/UX:
- Hiển thị progress khi upload (FileReader hoặc XHR để có progress events). fetch() không có progress native.
- Khi transcription có thể mất thời gian, show spinner và cho phép huỷ request (AbortController).
- Nếu transcript dài/chuỗi có ký tự đặc biệt, dùng payload JSON an toàn (JSON.stringify) — tránh chèn trực tiếp vào URL.
- Xử lý lỗi từ backend: hiển thị message thân thiện và log chi tiết cho dev console.

Ví dụ FE — Node (curl-like flow, an toàn với các ký tự):

// script node (sử dụng node fetch >=18 hoặc node-fetch)
import fs from 'fs';
import fetch from 'node-fetch';

const API_BASE = 'http://localhost:8080';
const AUDIO = './test.wav';

async function run() {
  // upload
  const form = new FormData();
  form.append('file', fs.createReadStream(AUDIO));

  const res = await fetch(`${API_BASE}/api/whisper/transcribe`, { method: 'POST', body: form });
  const j = await res.json();
  const transcript = j?.data?.Text ?? j?.data?.text ?? j?.text;
  console.log('Transcript:', transcript);

  // send to text-to-sql
  const q = await fetch(`${API_BASE}/api/text-to-sql/query`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ naturalLanguage: transcript }) });
  const r = await q.json();
  console.log('SQL result:', r);
}

run().catch(console.error);

CURL / jq one-liner (terminal)

API_BASE=http://localhost:8080
AUDIO=/path/to/test.wav

# upload and extract transcript
TRANSCRIPT=$(curl -s -F "file=@${AUDIO}" "${API_BASE}/api/whisper/transcribe" | jq -r '.data.Text // .data.text // .text // empty')

# write JSON safely to file and post
TMP=$(mktemp)
jq -n --arg t "$TRANSCRIPT" '{ naturalLanguage: $t }' > "$TMP"
curl -s -X POST "${API_BASE}/api/text-to-sql/query" -H "Content-Type: application/json" --data-binary @"$TMP" | jq .
rm -f "$TMP"

Authentication
- Nếu API cần JWT: thêm header Authorization: Bearer <token> trong cả request upload và request text->sql.
- Store token an toàn (httpOnly cookie preferred).

Edge cases và debug tips
- Nếu Whisper trả về dạng khác (ví dụ text không trong data.Text), kiểm tra cấu trúc response (console.log) và điều chỉnh JS jq.
- Nếu lỗi 500 từ /api/text-to-sql/query: bật logs server (Development) để xem SQL/querries/generated output.
- Nếu SQL generation chứa câu lệnh nguy hiểm (INSERT/DELETE...), controller/service thường validate; FE chỉ hiển thị kết quả sau server đã kiểm duyệt.

Kiến nghị cải tiến backend (tùy chọn)
- Trả luôn transcription và SQL trong một endpoint "/api/whisper/transcribe-and-query" (server-side orchestration) để FE chỉ gọi một lần. Lợi: giảm roundtrip, quản lý thời gian timeout/credentials dễ hơn. Cấu trúc:
  - POST /api/whisper/transcribe-and-query (multipart/form-data file)
  - Server: upload -> transcribe -> generate SQL -> execute -> trả về { transcription, sql, rows }
- Bổ sung webhook hoặc SSE cho tác vụ dài (long-running) nếu mất nhiều thời gian.

Tệp .http mẫu (VSCode REST Client)
- Tôi đã thêm file `text-to-sql-flow.http` trong repo. Mở file đó và chạy các request (đã set @API_BASE = http://localhost:8080).

Kết luận / Next steps
- Đã cung cấp luồng FE đầy đủ + ví dụ thực thi (browser/node/curl).
- Muốn tôi: (pick) tạo một endpoint backend hợp nhất `/api/whisper/transcribe-and-query` và implement service orchestration (tự động gọi Whisper -> Gemini -> Dapper) ? Hoặc tạo 1 file JS client cụ thể cho project FE? Ghi chọn 1 trong các option trên để tôi làm tiếp.



