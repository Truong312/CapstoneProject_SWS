# Cloudinary API Documentation

**Base URL:** `http://localhost:8080/api/cloudinary`

**Controller:** `CloudinaryController.cs`

**Purpose:** Upload và quản lý ảnh/file trên Cloudinary CDN

**Authorization:** Chưa có `[Authorize]` - Tất cả endpoint đều public (cần bổ sung)

---

## 📋 Table of Contents
1. [Upload File](#1-upload-file)
2. [Delete File](#2-delete-file)

---

## 1. Upload File

**Upload file (ảnh/video) lên Cloudinary**

### Request
```http
POST /api/cloudinary/upload
Content-Type: multipart/form-data
Authorization: Bearer {token}
```

**Form Data:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| File | IFormFile | ✅ Yes | File cần upload (ảnh, video, pdf...) |
| Folder | string | ❌ No | Tên folder trên Cloudinary (default: "SWP391") |

**Supported File Types:**
- Images: JPG, PNG, GIF, WebP, SVG
- Videos: MP4, MOV, AVI
- Documents: PDF
- Max size: Tùy thuộc vào Cloudinary plan

### Example Request (cURL)
```bash
curl -X POST http://localhost:8080/api/cloudinary/upload \
  -H "Authorization: Bearer {token}" \
  -F "File=@/path/to/image.jpg" \
  -F "Folder=products"
```

### Example Request (JavaScript Fetch)
```javascript
const formData = new FormData();
formData.append('File', fileInput.files[0]);
formData.append('Folder', 'products');

fetch('http://localhost:8080/api/cloudinary/upload', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
})
.then(response => response.json())
.then(data => console.log(data));
```

### Example Request (Postman)
```
Method: POST
URL: http://localhost:8080/api/cloudinary/upload
Body: form-data
  - Key: File, Type: File, Value: [Select file]
  - Key: Folder, Type: Text, Value: products
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "publicId": "SWP391/products/abc123def456",
    "url": "http://res.cloudinary.com/demo/image/upload/v1699612345/SWP391/products/abc123def456.jpg",
    "secureUrl": "https://res.cloudinary.com/demo/image/upload/v1699612345/SWP391/products/abc123def456.jpg",
    "format": "jpg",
    "bytes": 245678
  },
  "message": "Upload thành công"
}
```

**Response Fields:**
| Field | Type | Description |
|-------|------|-------------|
| publicId | string | ID duy nhất của file trên Cloudinary (dùng để xóa) |
| url | string | URL public (HTTP) của file |
| secureUrl | string | URL secure (HTTPS) của file - **Nên dùng URL này** |
| format | string | Định dạng file (jpg, png, mp4...) |
| bytes | long | Kích thước file (bytes) |

### Response Error (400 Bad Request) - No File
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "File không được để trống"
}
```

### Response Error (400 Bad Request) - Invalid File Type
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Định dạng file không được hỗ trợ. Chỉ chấp nhận: JPG, PNG, GIF, WebP"
}
```

### Response Error (400 Bad Request) - File Too Large
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "File quá lớn. Kích thước tối đa: 10MB"
}
```

### Response Error (500 Internal Server Error) - Cloudinary Error
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 500,
  "data": null,
  "message": "Lỗi khi upload file lên Cloudinary. Vui lòng thử lại sau."
}
```

---

## 2. Delete File

**Xóa file khỏi Cloudinary**

### Request
```http
DELETE /api/cloudinary/delete/{publicId}
Authorization: Bearer {token}
```

**Path Parameters:**
- `publicId` (string, required) - Public ID của file cần xóa (lấy từ response khi upload)

**⚠️ Important:** 
- Public ID có thể chứa `/` (slash) nên cần encode URL
- Ví dụ: `SWP391/products/abc123` → `SWP391%2Fproducts%2Fabc123`

### Example Request (cURL)
```bash
# Public ID: SWP391/products/abc123def456
curl -X DELETE "http://localhost:8080/api/cloudinary/delete/SWP391%2Fproducts%2Fabc123def456" \
  -H "Authorization: Bearer {token}"
```

### Example Request (JavaScript)
```javascript
const publicId = "SWP391/products/abc123def456";
const encodedPublicId = encodeURIComponent(publicId);

fetch(`http://localhost:8080/api/cloudinary/delete/${encodedPublicId}`, {
  method: 'DELETE',
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

### Example Request (Direct - No encoding needed in path)
```http
DELETE /api/cloudinary/delete/SWP391/products/abc123def456
Authorization: Bearer {token}
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "publicId": "SWP391/products/abc123def456",
    "status": "ok"
  },
  "message": "Xóa file thành công"
}
```

**Response Fields:**
| Field | Type | Description |
|-------|------|-------------|
| publicId | string | Public ID của file đã xóa |
| status | string | Trạng thái xóa ("ok" = thành công) |

### Response Error (400 Bad Request) - File Not Found
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": {
    "publicId": "SWP391/products/abc123def456",
    "status": "not found"
  },
  "message": "Không tìm thấy file với publicId: SWP391/products/abc123def456"
}
```

### Response Error (400 Bad Request) - Invalid Public ID
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "PublicId không hợp lệ"
}
```

### Response Error (500 Internal Server Error)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 500,
  "data": null,
  "message": "Lỗi khi xóa file trên Cloudinary. Vui lòng thử lại sau."
}
```

---

## 🔄 Complete Workflow

### Workflow 1: Upload Product Image

```javascript
// Step 1: Upload ảnh sản phẩm
const uploadImage = async (file) => {
  const formData = new FormData();
  formData.append('File', file);
  formData.append('Folder', 'products');
  
  const response = await fetch('/api/cloudinary/upload', {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}` },
    body: formData
  });
  
  const result = await response.json();
  return result.data.secureUrl; // Lưu URL này vào Product.Image
};

// Step 2: Tạo sản phẩm với ảnh
const createProduct = async (productData, imageFile) => {
  const imageUrl = await uploadImage(imageFile);
  
  await fetch('/api/product', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      ...productData,
      image: imageUrl // Sử dụng secureUrl từ Cloudinary
    })
  });
};
```

### Workflow 2: Update Product Image

```javascript
// Update ảnh sản phẩm (xóa ảnh cũ, upload ảnh mới)
const updateProductImage = async (productId, oldPublicId, newImageFile) => {
  // Step 1: Xóa ảnh cũ trên Cloudinary
  if (oldPublicId) {
    await fetch(`/api/cloudinary/delete/${encodeURIComponent(oldPublicId)}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    });
  }
  
  // Step 2: Upload ảnh mới
  const newImageUrl = await uploadImage(newImageFile);
  
  // Step 3: Update product với ảnh mới
  await fetch(`/api/product/${productId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      image: newImageUrl
    })
  });
};
```

### Workflow 3: Delete Product (with image cleanup)

```javascript
// Xóa sản phẩm và ảnh trên Cloudinary
const deleteProduct = async (productId) => {
  // Step 1: Lấy thông tin sản phẩm
  const productResponse = await fetch(`/api/product/${productId}`);
  const product = await productResponse.json();
  
  // Step 2: Extract publicId từ image URL
  const imageUrl = product.image;
  const publicId = extractPublicIdFromUrl(imageUrl);
  
  // Step 3: Xóa ảnh trên Cloudinary
  if (publicId) {
    await fetch(`/api/cloudinary/delete/${encodeURIComponent(publicId)}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    });
  }
  
  // Step 4: Xóa sản phẩm
  await fetch(`/api/product/${productId}`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` }
  });
};

// Helper function: Extract publicId từ Cloudinary URL
const extractPublicIdFromUrl = (url) => {
  // URL: https://res.cloudinary.com/demo/image/upload/v1699612345/SWP391/products/abc123.jpg
  // PublicId: SWP391/products/abc123
  const matches = url.match(/\/upload\/(?:v\d+\/)?(.+)\.\w+$/);
  return matches ? matches[1] : null;
};
```

---

## 📊 Common Response Structure

### Success Response
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    // CloudinaryUploadResponseDto or CloudinaryDeleteResponseDto
  },
  "message": "Success message in Vietnamese"
}
```

### Error Response
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400 | 500,
  "data": null,
  "message": "Error message in Vietnamese"
}
```

---

## 🎯 Use Cases

### 1. Product Images
```
Folder: "products"
Usage: Product.Image field
Example: https://res.cloudinary.com/.../SWP391/products/laptop-dell-xps-15.jpg
```

### 2. User Avatars
```
Folder: "avatars"
Usage: User.ProfileImage field
Example: https://res.cloudinary.com/.../SWP391/avatars/user-123.jpg
```

### 3. Export Order Documents
```
Folder: "documents/export-orders"
Usage: Đính kèm hóa đơn, chứng từ
Example: https://res.cloudinary.com/.../SWP391/documents/export-orders/invoice-001.pdf
```

### 4. Import Order Documents
```
Folder: "documents/import-orders"
Usage: Đính kèm phiếu nhập, hợp đồng
Example: https://res.cloudinary.com/.../SWP391/documents/import-orders/contract-001.pdf
```

---

## 🔒 Security Recommendations

⚠️ **Current State:** Không có authorization

**Khuyến nghị bổ sung:**
```csharp
[Authorize] // Yêu cầu đăng nhập
public class CloudinaryController : ControllerBase
{
    [HttpPost("upload")]
    [Authorize] // Chỉ user đã login mới upload được
    [RequestSizeLimit(10_485_760)] // Limit 10MB
    public async Task<IActionResult> Upload([FromForm] CloudinaryUploadRequestDto request)
    {
        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest("Invalid file type");
        }
        
        // Validate file size
        if (request.File.Length > 10_485_760) // 10MB
        {
            return BadRequest("File too large");
        }
        
        var result = await _cloudinaryService.UploadAsync(request);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpDelete("delete/{publicId}")]
    [Authorize(Roles = "1")] // Chỉ Staff/Admin mới xóa được
    public async Task<IActionResult> Delete(string publicId)
    {
        var result = await _cloudinaryService.DeleteAsync(publicId);
        return StatusCode(result.StatusCode, result);
    }
}
```

---

## ⚠️ Important Notes

### 1. File Size Limits
- **Recommended:** Max 10MB cho ảnh
- **Video:** Max 100MB
- Configure trong `appsettings.json` hoặc `Program.cs`

### 2. Allowed File Types
```csharp
// Nên validate file type
var allowedImageTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
var allowedDocTypes = new[] { ".pdf", ".doc", ".docx" };
var allowedVideoTypes = new[] { ".mp4", ".mov", ".avi" };
```

### 3. Public ID Format
```
Default Folder: SWP391
Format: {Folder}/{SubFolder}/{UniqueId}
Examples:
  - SWP391/products/abc123
  - SWP391/avatars/user-456
  - SWP391/documents/export-orders/inv-789
```

### 4. URL Encoding
```javascript
// ĐÚNG: Encode publicId khi có dấu /
const publicId = "SWP391/products/abc123";
const encodedId = encodeURIComponent(publicId);
// Result: SWP391%2Fproducts%2Fabc123

// SAI: Không encode
DELETE /api/cloudinary/delete/SWP391/products/abc123
// → API sẽ hiểu sai path parameters
```

### 5. Image Optimization
Cloudinary hỗ trợ transform URL để tối ưu ảnh:
```
Original: https://res.cloudinary.com/.../image.jpg
Resized: https://res.cloudinary.com/.../w_300,h_300,c_fill/image.jpg
Thumbnail: https://res.cloudinary.com/.../w_150,h_150,c_thumb/image.jpg
Quality: https://res.cloudinary.com/.../q_auto:best/image.jpg
```

---

## 🧪 Testing Examples

### Test Upload - Success
```bash
curl -X POST http://localhost:8080/api/cloudinary/upload \
  -H "Authorization: Bearer eyJhbGc..." \
  -F "File=@test-image.jpg" \
  -F "Folder=test"
```

Expected Response:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "data": {
    "publicId": "SWP391/test/xyz789",
    "secureUrl": "https://res.cloudinary.com/.../test/xyz789.jpg",
    "format": "jpg",
    "bytes": 123456
  }
}
```

### Test Delete - Success
```bash
curl -X DELETE "http://localhost:8080/api/cloudinary/delete/SWP391%2Ftest%2Fxyz789" \
  -H "Authorization: Bearer eyJhbGc..."
```

Expected Response:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "data": {
    "publicId": "SWP391/test/xyz789",
    "status": "ok"
  }
}
```

---

## 📝 Testing Checklist

**Upload Endpoint:**
- [ ] Upload image (JPG, PNG, GIF) - success
- [ ] Upload with custom folder - success
- [ ] Upload without folder (use default) - success
- [ ] Upload invalid file type - 400 error
- [ ] Upload file too large - 400 error
- [ ] Upload without file - 400 error
- [ ] Upload without authentication - 401 error

**Delete Endpoint:**
- [ ] Delete existing file - success
- [ ] Delete non-existent file - 400 error
- [ ] Delete with invalid publicId - 400 error
- [ ] Delete without authentication - 401 error

**Integration Tests:**
- [ ] Upload → Get secureUrl → Save to Product
- [ ] Upload new → Delete old → Update Product
- [ ] Delete Product → Delete Cloudinary image
- [ ] Verify file actually uploaded to Cloudinary
- [ ] Verify file actually deleted from Cloudinary

---

## 🐛 Common Issues & Solutions

### Issue 1: "File too large"
```
Solution: Increase request size limit in Program.cs
services.Configure<FormOptions>(options => {
    options.MultipartBodyLengthLimit = 10_485_760; // 10MB
});
```

### Issue 2: "Public ID not found"
```
Cause: Sai format publicId hoặc file đã bị xóa
Solution: 
1. Check publicId format (SWP391/folder/id)
2. Verify file exists on Cloudinary dashboard
3. Encode publicId khi có dấu /
```

### Issue 3: "Upload failed"
```
Possible causes:
1. Cloudinary credentials sai (check appsettings.json)
2. Network timeout
3. Cloudinary quota limit exceeded
Solution: Check Cloudinary dashboard và logs
```

### Issue 4: "Cannot delete file"
```
Cause: File đang được sử dụng ở nơi khác
Solution: 
1. Check Product.Image references
2. Check foreign key constraints
3. Soft delete thay vì hard delete
```

---

## 💡 Best Practices

### 1. Always Use secureUrl
```javascript
// ✅ ĐÚNG
product.image = result.data.secureUrl; // HTTPS

// ❌ SAI
product.image = result.data.url; // HTTP (không bảo mật)
```

### 2. Clean Up Old Images
```javascript
// Khi update/delete product, nhớ xóa ảnh cũ trên Cloudinary
const updateProduct = async (productId, newImageFile) => {
  const oldProduct = await getProduct(productId);
  const oldPublicId = extractPublicIdFromUrl(oldProduct.image);
  
  // Delete old image
  if (oldPublicId) {
    await deleteCloudinaryImage(oldPublicId);
  }
  
  // Upload new image
  const newImageUrl = await uploadImage(newImageFile);
  await updateProductImage(productId, newImageUrl);
};
```

### 3. Use Descriptive Folders
```javascript
// ✅ ĐÚNG: Có cấu trúc rõ ràng
Folder: "products"
Folder: "products/laptops"
Folder: "products/accessories"
Folder: "users/avatars"
Folder: "documents/invoices"

// ❌ SAI: Không có tổ chức
Folder: "images"
Folder: "files"
```

### 4. Handle Errors Gracefully
```javascript
try {
  const result = await uploadImage(file);
  return result.data.secureUrl;
} catch (error) {
  console.error('Upload failed:', error);
  // Fallback to default image
  return '/images/default-product.jpg';
}
```

### 5. Add File Metadata
```javascript
// Lưu thêm metadata cho dễ tracking
const uploadWithMetadata = async (file, metadata) => {
  const formData = new FormData();
  formData.append('File', file);
  formData.append('Folder', `products/${metadata.category}`);
  
  const result = await upload(formData);
  
  // Save metadata to database
  await saveImageMetadata({
    publicId: result.data.publicId,
    url: result.data.secureUrl,
    productId: metadata.productId,
    uploadedBy: metadata.userId,
    uploadedAt: new Date()
  });
  
  return result.data.secureUrl;
};
```

---

## 🔗 Related APIs

**Cloudinary API được sử dụng với:**
- **Product API** - Upload ảnh sản phẩm (`Product.Image`)
- **User API** - Upload avatar (`User.ProfileImage`)
- **Export Order API** - Upload hóa đơn, chứng từ
- **Import Order API** - Upload phiếu nhập, hợp đồng

**Dependency Flow:**
```
Frontend → CloudinaryController → CloudinaryService → Cloudinary SDK → Cloudinary CDN
                                                              ↓
                                                      Return URL
                                                              ↓
                            Save URL to Database (Product.Image, User.Avatar...)
```

