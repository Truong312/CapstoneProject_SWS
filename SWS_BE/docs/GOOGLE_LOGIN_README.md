# Google Login Authentication - Documentation

## Tổng quan

Hệ thống Google Login sử dụng OAuth 2.0 để xác thực người dùng thông qua tài khoản Google của họ.

## Kiến trúc Clean Code

### 1. **DTOs (BusinessObjects/Dtos/GoogleAuthDto.cs)**
- `GoogleLoginRequestDto`: Request từ client
- `GoogleUserInfoDto`: Thông tin user từ Google
- `GoogleLoginResponseDto`: Response trả về sau khi login thành công
- `GoogleAuthUrlDto`: URL để redirect tới Google

### 2. **Mapping (Services/Mappings/MappingProfile.cs)**
- Map `GoogleUserInfo` (ApiModel) → `GoogleUserInfoDto` (DTO)
- Map `User` (Entity) → `GoogleLoginResponseDto` (DTO)
- Tách biệt giữa API models và DTOs để dễ bảo trì

### 3. **Services Layer**
- `IGoogleLoginService`: Interface cho Google Login
- `GoogleLoginService`: Xử lý OAuth flow với Google
- `IWarehouseAuthenticationService`: Interface cho authentication
- `WarehouseAuthenticationService`: Xử lý business logic

### 4. **Controller (ApiCore/Controllers/WarehouseAuthController.cs)**
- Expose APIs cho client
- Validation đầu vào
- Trả về response theo chuẩn

## Flow đăng nhập Google

```
┌─────────┐         ┌─────────┐         ┌──────────────┐         ┌────────┐
│ Client  │         │   API   │         │   Service    │         │ Google │
└────┬────┘         └────┬────┘         └──────┬───────┘         └────┬───┘
     │                   │                     │                      │
     │ 1. GET google-url │                     │                      │
     ├──────────────────>│                     │                      │
     │                   │  GetGoogleLoginUrl()│                      │
     │                   ├────────────────────>│                      │
     │                   │  Return auth URL    │                      │
     │                   │<────────────────────┤                      │
     │  Return auth URL  │                     │                      │
     │<──────────────────┤                     │                      │
     │                   │                     │                      │
     │ 2. Redirect to Google OAuth URL                                │
     ├───────────────────────────────────────────────────────────────>│
     │                   │                     │                      │
     │ 3. User login & consent                 │                      │
     │<───────────────────────────────────────────────────────────────┤
     │                   │                     │                      │
     │ 4. Redirect with code                   │                      │
     │<───────────────────────────────────────────────────────────────┤
     │                   │                     │                      │
     │ 5. POST google-login {code}             │                      │
     ├──────────────────>│                     │                      │
     │                   │ LoginWithGoogleAsync│                      │
     │                   ├────────────────────>│                      │
     │                   │                     │ Exchange code        │
     │                   │                     ├─────────────────────>│
     │                   │                     │ Return access token  │
     │                   │                     │<─────────────────────┤
     │                   │                     │ Get user info        │
     │                   │                     ├─────────────────────>│
     │                   │                     │ Return user info     │
     │                   │                     │<─────────────────────┤
     │                   │                     │                      │
     │                   │   Create/Get User   │                      │
     │                   │   Generate JWT      │                      │
     │                   │   Return response   │                      │
     │                   │<────────────────────┤                      │
     │  JWT + User Info  │                     │                      │
     │<──────────────────┤                     │                      │
     │                   │                     │                      │
```

## API Endpoints

### 1. Lấy URL đăng nhập Google
```http
GET /api/warehouse/auth/google-url
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Lấy URL đăng nhập Google thành công",
  "data": {
    "authUrl": "https://accounts.google.com/o/oauth2/v2/auth?client_id=..."
  }
}
```

### 2. Đăng nhập bằng Google
```http
POST /api/warehouse/auth/google-login
Content-Type: application/json

{
  "code": "4/0AeanS0abcdefghijklmnopqrstuvwxyz1234567890"
}
```

**Response (User mới):**
```json
{
  "isSuccess": true,
  "message": "Đăng ký và đăng nhập bằng Google thành công",
  "data": {
    "userId": 1,
    "fullName": "Nguyen Van A",
    "email": "nguyenvana@gmail.com",
    "phone": null,
    "address": null,
    "role": 0,
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "isNewUser": true
  },
  "statusCode": 200
}
```

**Response (User đã tồn tại):**
```json
{
  "isSuccess": true,
  "message": "Đăng nhập bằng Google thành công",
  "data": {
    "userId": 1,
    "fullName": "Nguyen Van A",
    "email": "nguyenvana@gmail.com",
    "phone": "0123456789",
    "address": "Ha Noi",
    "role": 0,
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "isNewUser": false
  },
  "statusCode": 200
}
```

## Configuration

### appsettings.json
```json
{
  "GoogleAuthSettings": {
    "ClientId": "your-client-id.apps.googleusercontent.com",
    "ClientSecret": "your-client-secret",
    "RedirectUri": "http://localhost:3000/auth/callback"
  }
}
```

### GoogleAuthSettings.cs
```csharp
public class GoogleAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
```

## Cấu hình Google Console

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Enable Google+ API
4. Tạo OAuth 2.0 credentials:
   - Application type: Web application
   - Authorized redirect URIs: `http://localhost:3000/auth/callback`
5. Copy Client ID và Client Secret vào appsettings.json

## Testing với file .http

Sử dụng file `GoogleAuth.http` để test:

```http
### 1. Lấy URL đăng nhập Google
GET https://localhost:7001/api/warehouse/auth/google-url

### 2. Đăng nhập bằng Google
POST https://localhost:7001/api/warehouse/auth/google-login
Content-Type: application/json

{
  "code": "YOUR_AUTHORIZATION_CODE"
}
```

## Frontend Integration

### React Example
```typescript
// 1. Lấy Google Auth URL
const getGoogleAuthUrl = async () => {
  const response = await fetch('https://localhost:7001/api/warehouse/auth/google-url');
  const data = await response.json();
  window.location.href = data.data.authUrl;
};

// 2. Handle callback
const handleGoogleCallback = async () => {
  const params = new URLSearchParams(window.location.search);
  const code = params.get('code');
  
  if (code) {
    const response = await fetch('https://localhost:7001/api/warehouse/auth/google-login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ code })
    });
    
    const result = await response.json();
    
    if (result.isSuccess) {
      // Lưu token
      localStorage.setItem('token', result.data.token);
      localStorage.setItem('user', JSON.stringify(result.data));
      
      // Redirect to dashboard
      window.location.href = '/dashboard';
    }
  }
};
```

## Error Handling

### Common Errors

1. **Invalid Code**
```json
{
  "isSuccess": false,
  "message": "Lỗi khi đăng nhập bằng Google: Google token request failed...",
  "statusCode": 500
}
```

2. **Missing Code**
```json
{
  "isSuccess": false,
  "message": "Mã code từ Google là bắt buộc"
}
```

3. **Google API Error**
```json
{
  "isSuccess": false,
  "message": "Không thể lấy thông tin từ Google",
  "statusCode": 400
}
```

## Security Notes

- ✅ Sử dụng HTTPS trong production
- ✅ Validate authorization code
- ✅ Generate random password cho Google users
- ✅ JWT token có thời hạn 24 giờ
- ✅ Không lưu Google access token
- ✅ Sử dụng DTOs để tách biệt layers

## Cấu trúc Clean Architecture

```
SWS.BusinessObjects/Dtos/
├── GoogleAuthDto.cs (DTOs cho Google Login)

SWS.Services/
├── ApiModels/AccountModel/
│   └── GoogleUserInfo.cs (API response model từ Google)
├── Mappings/
│   └── MappingProfile.cs (AutoMapper configuration)
└── Services/WarehouseAuthentication/
    ├── IGoogleLoginService.cs
    ├── GoogleLoginService.cs
    ├── IWarehouseAuthenticationService.cs
    └── WarehouseAuthenticationService.cs

SWS.ApiCore/Controllers/
└── WarehouseAuthController.cs (API endpoints)
```

## Lợi ích của Clean Code

1. **Separation of Concerns**: DTOs, Services, Controllers tách biệt rõ ràng
2. **Maintainability**: Dễ bảo trì và mở rộng
3. **Testability**: Dễ dàng viết unit tests
4. **Reusability**: DTOs có thể sử dụng ở nhiều nơi
5. **Type Safety**: Strongly typed với C#
6. **AutoMapper**: Tự động mapping giữa các models

## Next Steps

- [ ] Implement refresh token
- [ ] Add rate limiting cho Google login
- [ ] Log Google login events
- [ ] Add email verification for new users
- [ ] Implement social login with Facebook, GitHub

