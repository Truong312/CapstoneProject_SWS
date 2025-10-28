# 🔧 Hướng dẫn Fix Lỗi: redirect_uri_mismatch

## ❌ Lỗi hiện tại:
```
Error 400: redirect_uri_mismatch
```

## ✅ Nguyên nhân:
Google Console chưa có URL redirect mới: `http://localhost:8080/api/warehouse/auth/google-callback`

---

## 📋 BƯỚC FIX (QUAN TRỌNG):

### Bước 1: Truy cập Google Cloud Console
1. Mở https://console.cloud.google.com/
2. Đăng nhập với tài khoản đã tạo OAuth Client

### Bước 2: Chọn Project
1. Chọn project "HotelManagement" (hoặc project của bạn)
2. Vào menu bên trái: **APIs & Services** → **Credentials**

### Bước 3: Edit OAuth 2.0 Client ID
1. Tìm Client ID: `166370023031-5fb6unqprsf9f020f1n0cvhk333kdbj4.apps.googleusercontent.com`
2. Click vào nó để edit
3. Hoặc click icon **✏️ (Edit)** bên cạnh

### Bước 4: Thêm Authorized redirect URIs
Trong phần **Authorized redirect URIs**, thêm các URL sau:

```
http://localhost:8080/api/warehouse/auth/google-callback
```

**XÓA URL cũ (nếu có):**
```
http://localhost:8080/api/Authentication/callback-google
```

### Bước 5: Thêm Authorized JavaScript origins
Trong phần **Authorized JavaScript origins**, đảm bảo có:

```
http://localhost:8080
http://localhost:3000
```

### Bước 6: Save
1. Click **SAVE** ở cuối trang
2. Đợi vài giây để Google cập nhật

---

## 🧪 Test lại:

### Cách 1: Test trên Browser
1. Mở browser
2. Truy cập: http://localhost:8080/api/warehouse/auth/google-url
3. Copy `authUrl` từ response
4. Paste vào browser và Enter
5. Đăng nhập Google

### Cách 2: Test với file .http
```http
GET http://localhost:8080/api/warehouse/auth/google-url
```

Lấy URL từ response và mở trong browser.

---

## 📸 Screenshot Cấu hình Google Console:

### Ví dụ cấu hình đúng:

**Authorized redirect URIs:**
```
✅ http://localhost:8080/api/warehouse/auth/google-callback
```

**Authorized JavaScript origins:**
```
✅ http://localhost:8080
✅ http://localhost:3000
```

---

## 🔍 Kiểm tra lại cấu hình hiện tại:

### Backend (appsettings.json):
```json
{
  "GoogleAuthSettings": {
    "ClientId": "166370023031-5fb6unqprsf9f020f1n0cvhk333kdbj4.apps.googleusercontent.com",
    "ClientSecret": "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",
    "RedirectUri": "http://localhost:8080/api/warehouse/auth/google-callback"
  }
}
```

### Controller endpoint:
```csharp
[HttpGet("google-callback")]
[Route("/api/warehouse/auth/google-callback")]
```

---

## 🚀 Flow hoàn chỉnh sau khi fix:

```
1. User click "Login with Google"
   ↓
2. Frontend gọi: GET /api/warehouse/auth/google-url
   ↓
3. Response trả về authUrl, frontend redirect user tới URL đó
   ↓
4. User đăng nhập Google và đồng ý
   ↓
5. Google redirect về: http://localhost:8080/api/warehouse/auth/google-callback?code=xxx
   ↓
6. Backend nhận code, gọi Google API, tạo/lấy user, tạo JWT
   ↓
7. Backend redirect về: http://localhost:3000/auth/callback?token=xxx&isNewUser=true
   ↓
8. Frontend lưu token vào localStorage
   ↓
9. Done! ✅
```

---

## ⚠️ Lưu ý:

1. **Sau khi update Google Console, đợi 1-2 phút** trước khi test lại
2. **Clear browser cache** nếu vẫn gặp lỗi
3. **Kiểm tra chính xác URL** - không có dấu `/` thừa ở cuối
4. **Port phải khớp** - backend đang chạy ở port 8080

---

## 🆘 Nếu vẫn lỗi:

### Kiểm tra URL đang được gửi:
1. Gọi API: `GET /api/warehouse/auth/google-url`
2. Check `authUrl` trong response
3. Xem `redirect_uri` parameter có đúng không:
   ```
   redirect_uri=http://localhost:8080/api/warehouse/auth/google-callback
   ```

### Double check Google Console:
1. Vào Credentials
2. Click vào OAuth 2.0 Client ID
3. Scroll xuống phần "Authorized redirect URIs"
4. Đảm bảo có đúng URL: `http://localhost:8080/api/warehouse/auth/google-callback`
5. Click Save lại

---

## ✨ Sau khi fix xong:

Bạn có thể test bằng cách:

1. **Restart backend** (quan trọng!)
```bash
# Stop server (Ctrl + C)
# Start lại server
dotnet run --project SWS.ApiCore
```

2. **Test với Postman hoặc .http file:**
```http
GET http://localhost:8080/api/warehouse/auth/google-url
```

3. **Copy authUrl và paste vào browser**

4. **Đăng nhập Google**

5. **Kiểm tra redirect về frontend**

---

Chúc bạn thành công! 🎉

