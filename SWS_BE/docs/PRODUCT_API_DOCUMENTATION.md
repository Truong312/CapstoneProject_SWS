# Product API Documentation

**Base URL:** `http://localhost:8080/api/product`

**Controller:** `ProductController.cs`

**Authorization:** Chưa có `[Authorize]` - Tất cả endpoint đều public (cần bổ sung)

---

## 📋 Table of Contents
1. [Get All Products](#1-get-all-products)
2. [Get Product By ID](#2-get-product-by-id)
3. [Create Product](#3-create-product)
4. [Update Product](#4-update-product)
5. [Delete Product](#5-delete-product)
6. [Get Near Expired Products](#6-get-near-expired-products)
7. [Get Expired Products](#7-get-expired-products)
8. [Get Low Stock Products](#8-get-low-stock-products)
9. [Search Products](#9-search-products)
10. [Get Products Paged](#10-get-products-paged)

---

## 1. Get All Products

**Lấy danh sách tất cả sản phẩm**

### Request
```http
GET /api/product
Accept: application/json
```

### Response Success (200 OK)
```json
[
  {
    "productId": 1,
    "serialNumber": "PROD-001",
    "name": "Laptop Dell XPS 15",
    "expiredDate": "2026-12-31",
    "unit": "Cái",
    "unitPrice": 25000000,
    "receivedDate": "2025-11-10",
    "purchasedPrice": 20000000,
    "reorderPoint": 5,
    "image": "https://example.com/images/dell-xps-15.jpg",
    "description": "Laptop cao cấp cho doanh nhân"
  },
  {
    "productId": 2,
    "serialNumber": "PROD-002",
    "name": "Mouse Logitech MX Master 3",
    "expiredDate": "2027-06-30",
    "unit": "Cái",
    "unitPrice": 2500000,
    "receivedDate": "2025-11-08",
    "purchasedPrice": 2000000,
    "reorderPoint": 10,
    "image": "https://example.com/images/mx-master-3.jpg",
    "description": "Chuột không dây cao cấp"
  }
]
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Lỗi khi lấy danh sách sản phẩm"
}
```

---

## 2. Get Product By ID

**Lấy chi tiết một sản phẩm theo ID**

### Request
```http
GET /api/product/{id}
Accept: application/json
```

**Path Parameters:**
- `id` (integer, required) - ID của sản phẩm

**Example:**
```http
GET /api/product/1
```

### Response Success (200 OK)
```json
{
  "productId": 1,
  "serialNumber": "PROD-001",
  "name": "Laptop Dell XPS 15",
  "expiredDate": "2026-12-31",
  "unit": "Cái",
  "unitPrice": 25000000,
  "receivedDate": "2025-11-10",
  "purchasedPrice": 20000000,
  "reorderPoint": 5,
  "image": "https://example.com/images/dell-xps-15.jpg",
  "description": "Laptop cao cấp cho doanh nhân"
}
```

### Response Error (404 Not Found)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 404,
  "data": null,
  "message": "Không tìm thấy sản phẩm với ID = 1"
}
```

---

## 3. Create Product

**Tạo sản phẩm mới**

### Request
```http
POST /api/product
Content-Type: application/json
```

**Request Body:**
```json
{
  "serialNumber": "PROD-003",
  "name": "Bàn phím cơ Keychron K8",
  "expiredDate": "2027-12-31",
  "unit": "Cái",
  "unitPrice": 3200000,
  "receivedDate": "2025-11-10",
  "purchasedPrice": 2500000,
  "reorderPoint": 8,
  "image": "https://example.com/images/keychron-k8.jpg",
  "description": "Bàn phím cơ hot-swap"
}
```

**Field Descriptions:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| serialNumber | string | ✅ Yes | Mã số Serial của sản phẩm |
| name | string | ✅ Yes | Tên sản phẩm |
| expiredDate | string (DateOnly) | ✅ Yes | Ngày hết hạn (yyyy-MM-dd) |
| unit | string | ❌ No | Đơn vị tính (hộp, chai, kg...) |
| unitPrice | decimal | ❌ No | Giá bán (VNĐ) |
| receivedDate | string (DateOnly) | ✅ Yes | Ngày nhập kho (yyyy-MM-dd) |
| purchasedPrice | decimal | ❌ No | Giá mua vào (VNĐ) |
| reorderPoint | integer | ❌ No | Điểm đặt hàng lại |
| image | string | ❌ No | URL ảnh sản phẩm |
| description | string | ❌ No | Mô tả sản phẩm |

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "productId": 3,
    "serialNumber": "PROD-003",
    "name": "Bàn phím cơ Keychron K8",
    "expiredDate": "2027-12-31",
    "unit": "Cái",
    "unitPrice": 3200000,
    "receivedDate": "2025-11-10",
    "purchasedPrice": 2500000,
    "reorderPoint": 8,
    "image": "https://example.com/images/keychron-k8.jpg",
    "description": "Bàn phím cơ hot-swap"
  },
  "message": "Tạo sản phẩm thành công"
}
```

### Response Error (400 Bad Request) - Validation Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ],
    "SerialNumber": [
      "The SerialNumber field is required."
    ]
  }
}
```

### Response Error (400 Bad Request) - Business Logic Error
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Mã serial PROD-003 đã tồn tại"
}
```

---

## 4. Update Product

**Cập nhật thông tin sản phẩm**

### Request
```http
PUT /api/product/{id}
Content-Type: application/json
```

**Path Parameters:**
- `id` (integer, required) - ID của sản phẩm cần update

**Request Body:**
```json
{
  "name": "Laptop Dell XPS 15 Gen 2024",
  "expiredDate": "2027-12-31",
  "unit": "Cái",
  "unitPrice": 27000000,
  "receivedDate": "2025-11-10",
  "purchasedPrice": 22000000,
  "reorderPoint": 10,
  "image": "https://example.com/images/dell-xps-15-gen2024.jpg",
  "description": "Laptop cao cấp phiên bản 2024"
}
```

**Field Descriptions:** (Tất cả đều optional - chỉ gửi field muốn update)
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | ❌ No | Tên sản phẩm mới |
| expiredDate | string (DateOnly) | ❌ No | Ngày hết hạn mới |
| unit | string | ❌ No | Đơn vị tính mới |
| unitPrice | decimal | ❌ No | Giá bán mới |
| receivedDate | string (DateOnly) | ❌ No | Ngày nhập kho mới |
| purchasedPrice | decimal | ❌ No | Giá mua mới |
| reorderPoint | integer | ❌ No | Điểm đặt hàng lại mới |
| image | string | ❌ No | URL ảnh mới |
| description | string | ❌ No | Mô tả mới |

**Example:**
```http
PUT /api/product/1
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "productId": 1,
    "serialNumber": "PROD-001",
    "name": "Laptop Dell XPS 15 Gen 2024",
    "expiredDate": "2027-12-31",
    "unit": "Cái",
    "unitPrice": 27000000,
    "receivedDate": "2025-11-10",
    "purchasedPrice": 22000000,
    "reorderPoint": 10,
    "image": "https://example.com/images/dell-xps-15-gen2024.jpg",
    "description": "Laptop cao cấp phiên bản 2024"
  },
  "message": "Cập nhật sản phẩm thành công"
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Không tìm thấy sản phẩm với ID = 1"
}
```

---

## 5. Delete Product

**Xóa sản phẩm**

### Request
```http
DELETE /api/product/{id}
```

**Path Parameters:**
- `id` (integer, required) - ID của sản phẩm cần xóa

**Example:**
```http
DELETE /api/product/3
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": null,
  "message": "Xóa sản phẩm thành công"
}
```

### Response Error (400 Bad Request) - Product Not Found
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Không tìm thấy sản phẩm với ID = 3"
}
```

### Response Error (400 Bad Request) - Has Dependencies
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Không thể xóa sản phẩm vì đã có giao dịch Import/Export liên quan"
}
```

**⚠️ Important Notes:**
- Nên kiểm tra xem sản phẩm có giao dịch Import/Export trước khi xóa
- Khuyến nghị: Sử dụng soft delete (isActive = false) thay vì hard delete

---

## 6. Get Near Expired Products

**Lấy danh sách sản phẩm sắp hết hạn (< 30 ngày)**

### Request
```http
GET /api/product/near-expired
Accept: application/json
```

### Response Success (200 OK)
```json
[
  {
    "productId": 5,
    "serialNumber": "PROD-005",
    "name": "Sữa tươi Vinamilk",
    "expiredDate": "2025-12-01",
    "unit": "Hộp",
    "unitPrice": 35000,
    "receivedDate": "2025-11-01",
    "purchasedPrice": 28000,
    "reorderPoint": 50,
    "image": "https://example.com/images/vinamilk.jpg",
    "description": "Sữa tươi tiệt trùng"
  },
  {
    "productId": 6,
    "serialNumber": "PROD-006",
    "name": "Bánh quy Oreo",
    "expiredDate": "2025-11-25",
    "unit": "Gói",
    "unitPrice": 25000,
    "receivedDate": "2025-10-01",
    "purchasedPrice": 18000,
    "reorderPoint": 30,
    "image": "https://example.com/images/oreo.jpg",
    "description": "Bánh quy sandwich"
  }
]
```

### Response Error (404 Not Found)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 404,
  "data": null,
  "message": "Không có sản phẩm nào sắp hết hạn"
}
```

**Business Logic:**
- Sản phẩm có `expiredDate` trong khoảng từ hôm nay đến 30 ngày tới
- Sắp xếp theo `expiredDate` tăng dần (sắp hết hạn nhất lên đầu)

---

## 7. Get Expired Products

**Lấy danh sách sản phẩm đã hết hạn**

### Request
```http
GET /api/product/expired
Accept: application/json
```

### Response Success (200 OK)
```json
[
  {
    "productId": 7,
    "serialNumber": "PROD-007",
    "name": "Nước ngọt Coca Cola",
    "expiredDate": "2025-11-05",
    "unit": "Chai",
    "unitPrice": 15000,
    "receivedDate": "2025-09-01",
    "purchasedPrice": 12000,
    "reorderPoint": 100,
    "image": "https://example.com/images/coca.jpg",
    "description": "Nước giải khát có gas"
  }
]
```

### Response Error (404 Not Found)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 404,
  "data": null,
  "message": "Không có sản phẩm nào đã hết hạn"
}
```

**Business Logic:**
- Sản phẩm có `expiredDate < Today` (2025-11-10)
- 🚫 Không nên xuất kho các sản phẩm này
- Nên tạo Return Order hoặc Adjustment để loại bỏ

---

## 8. Get Low Stock Products

**Lấy danh sách sản phẩm tồn kho thấp**

### Request
```http
GET /api/product/low-stock
Accept: application/json
```

### Response Success (200 OK)
```json
[
  {
    "productId": 1,
    "serialNumber": "PROD-001",
    "name": "Laptop Dell XPS 15",
    "expiredDate": "2026-12-31",
    "unit": "Cái",
    "unitPrice": 25000000,
    "receivedDate": "2025-11-10",
    "purchasedPrice": 20000000,
    "reorderPoint": 5,
    "image": "https://example.com/images/dell-xps-15.jpg",
    "description": "Laptop cao cấp cho doanh nhân"
  }
]
```

### Response Error (404 Not Found)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 404,
  "data": null,
  "message": "Không có sản phẩm nào tồn kho thấp"
}
```

**Business Logic:**
- Sản phẩm có `QuantityInStock < ReorderPoint`
- 🔔 Cần đặt hàng bổ sung tồn kho
- Trigger notification cho staff

---

## 9. Search Products

**Tìm kiếm sản phẩm theo từ khóa**

### Request
```http
GET /api/product/search?text={keyword}
Accept: application/json
```

**Query Parameters:**
- `text` (string, required) - Từ khóa tìm kiếm

**Example:**
```http
GET /api/product/search?text=laptop
```

### Response Success (200 OK)
```json
[
  {
    "productId": 1,
    "serialNumber": "PROD-001",
    "name": "Laptop Dell XPS 15",
    "expiredDate": "2026-12-31",
    "unit": "Cái",
    "unitPrice": 25000000,
    "receivedDate": "2025-11-10",
    "purchasedPrice": 20000000,
    "reorderPoint": 5,
    "image": "https://example.com/images/dell-xps-15.jpg",
    "description": "Laptop cao cấp cho doanh nhân"
  },
  {
    "productId": 8,
    "serialNumber": "PROD-008",
    "name": "Laptop HP Pavilion",
    "expiredDate": "2027-06-30",
    "unit": "Cái",
    "unitPrice": 18000000,
    "receivedDate": "2025-11-08",
    "purchasedPrice": 15000000,
    "reorderPoint": 3,
    "image": "https://example.com/images/hp-pavilion.jpg",
    "description": "Laptop phổ thông"
  }
]
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Từ khóa tìm kiếm không được để trống"
}
```

**Search Fields:**
- `Name` (tên sản phẩm)
- `SerialNumber` (mã serial)
- `Description` (mô tả)

**Search Logic:** Case-insensitive, LIKE '%keyword%'

---

## 10. Get Products Paged

**Lấy danh sách sản phẩm có phân trang và tìm kiếm**

### Request
```http
GET /api/product/paged?page={page}&pageSize={pageSize}&q={keyword}
Accept: application/json
```

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| page | integer | ❌ No | 1 | Trang hiện tại (1-based) |
| pageSize | integer | ❌ No | 20 | Số lượng items mỗi trang |
| q | string | ❌ No | null | Từ khóa tìm kiếm |

**Examples:**
```http
GET /api/product/paged
GET /api/product/paged?page=2
GET /api/product/paged?page=1&pageSize=10
GET /api/product/paged?page=1&pageSize=20&q=laptop
```

### Response Success (200 OK)
```json
{
  "totalItems": 125,
  "page": 1,
  "pageSize": 20,
  "totalPages": 7,
  "items": [
    {
      "productId": 1,
      "serialNumber": "PROD-001",
      "name": "Laptop Dell XPS 15",
      "expiredDate": "2026-12-31",
      "unit": "Cái",
      "unitPrice": 25000000,
      "receivedDate": "2025-11-10",
      "purchasedPrice": 20000000,
      "reorderPoint": 5,
      "image": "https://example.com/images/dell-xps-15.jpg",
      "description": "Laptop cao cấp cho doanh nhân"
    },
    {
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Trang phải lớn hơn 0"
}
```

**Pagination Calculation:**
```
totalPages = Math.Ceiling(totalItems / pageSize)
offset = (page - 1) * pageSize
```

---

## 📊 Common Response Structure

Tất cả các endpoint sử dụng `ResultModel<T>` hoặc trực tiếp trả về data:

### Success Response (when returning ResultModel)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": { /* ProductResponseDto or array */ },
  "message": "Success message"
}
```

### Error Response
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400 | 404,
  "data": null,
  "message": "Error message"
}
```

---

## 🔒 Security Recommendations

⚠️ **Current State:** Không có authorization trên controller

**Khuyến nghị bổ sung:**
```csharp
[Authorize] // Yêu cầu đăng nhập
public class ProductController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts() { }
    
    [HttpPost]
    [Authorize(Roles = "1")] // Chỉ Staff mới tạo được
    public async Task<IActionResult> CreateProduct() { }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "1")] // Chỉ Staff mới update được
    public async Task<IActionResult> UpdateProduct() { }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "1")] // Chỉ Staff mới xóa được
    public async Task<IActionResult> DeleteProduct() { }
}
```

---

## 🎯 Business Rules Summary

1. **Create Product:**
   - SerialNumber phải unique
   - ExpiredDate phải > ReceivedDate
   - Giá bán nên >= Giá mua

2. **Update Product:**
   - Không update SerialNumber (immutable)
   - Thay đổi giá không ảnh hưởng đơn hàng cũ
   - Thay đổi ReorderPoint → check lại low-stock

3. **Delete Product:**
   - Kiểm tra Import/Export Details trước khi xóa
   - Khuyến nghị soft delete thay vì hard delete

4. **Low Stock:**
   - Alert khi QuantityInStock < ReorderPoint
   - Trigger notification cho staff

5. **Expired Products:**
   - Không cho phép Export sản phẩm hết hạn
   - Cần tạo Return/Adjustment Order để loại bỏ

---

## 📝 Testing Checklist

- [ ] GET all products
- [ ] GET product by valid ID
- [ ] GET product by invalid ID (404)
- [ ] POST create product (success)
- [ ] POST create product with duplicate serial (400)
- [ ] POST create product with missing required fields (400)
- [ ] PUT update product (success)
- [ ] PUT update non-existent product (404)
- [ ] DELETE product without dependencies (success)
- [ ] DELETE product with dependencies (400)
- [ ] GET near-expired products
- [ ] GET expired products
- [ ] GET low-stock products
- [ ] GET search with valid keyword
- [ ] GET search with empty keyword
- [ ] GET paged with different page numbers
- [ ] GET paged with different page sizes
- [ ] GET paged with search keyword
      "productId": 2,
      "serialNumber": "PROD-002",
      "name": "Mouse Logitech MX Master 3",
      "expiredDate": "2027-06-30",
      "unit": "Cái",
      "unitPrice": 2500000,
      "receivedDate": "2025-11-08",
      "purchasedPrice": 2000000,
      "reorderPoint": 10,
      "image": "https://example.com/images/mx-master-3.jpg",
      "description": "Chuột không dây cao cấp"
    }
  ]
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,

