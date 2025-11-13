# Export Order API Documentation

**Base URL:** `http://localhost:8080/api/ExportOrder`

**Controller:** `ExportOrderController.cs`

**Authorization:** Chưa có `[Authorize]` - Tất cả endpoint đều public (cần bổ sung)

---

## 📋 Table of Contents

### Export Orders (Header)
1. [Get All Export Orders](#1-get-all-export-orders)
2. [Get Export Orders by Status](#2-get-export-orders-by-status)
3. [Create Export Order](#3-create-export-order)
4. [Update Export Order](#4-update-export-order)
5. [Delete Export Order](#5-delete-export-order)

### Export Details (Lines)
6. [Get Export Order Details](#6-get-export-order-details)
7. [Create Export Detail](#7-create-export-detail)
8. [Update Export Detail](#8-update-export-detail)
9. [Delete Export Detail](#9-delete-export-detail)

---

# EXPORT ORDERS (HEADER)

## 1. Get All Export Orders

**Lấy tất cả đơn xuất kho**

### Request
```http
GET /api/ExportOrder/All
Accept: application/json
Authorization: Bearer {token}
```

### Response Success (200 OK)
```json
[
  {
    "exportOrderId": 1,
    "invoiceNumber": "EXP-20251110-001",
    "orderDate": "2025-11-10",
    "customerId": 10,
    "currency": "VND",
    "createdDate": "2025-11-10",
    "shippedDate": "2025-11-12",
    "shippedAddress": "123 Đường Láng, Đống Đa, Hà Nội",
    "taxRate": 0.10,
    "taxAmount": 5000000,
    "totalPayment": 55000000,
    "description": "Đơn hàng xuất kho tháng 11",
    "status": 0,
    "createdBy": 1
  },
  {
    "exportOrderId": 2,
    "invoiceNumber": "EXP-20251109-002",
    "orderDate": "2025-11-09",
    "customerId": 15,
    "currency": "VND",
    "createdDate": "2025-11-09",
    "shippedDate": "2025-11-11",
    "shippedAddress": "456 Nguyễn Trãi, Thanh Xuân, Hà Nội",
    "taxRate": 0.10,
    "taxAmount": 3000000,
    "totalPayment": 33000000,
    "description": "Xuất hàng cho khách VIP",
    "status": 1,
    "createdBy": 1
  },
  {
    "exportOrderId": 3,
    "invoiceNumber": "EXP-20251108-003",
    "orderDate": "2025-11-08",
    "customerId": 20,
    "currency": "VND",
    "createdDate": "2025-11-08",
    "shippedDate": "2025-11-10",
    "shippedAddress": "789 Giải Phóng, Hai Bà Trưng, Hà Nội",
    "taxRate": 0.08,
    "taxAmount": 1600000,
    "totalPayment": 21600000,
    "description": null,
    "status": 2,
    "createdBy": 2
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
  "message": "Không có đơn xuất kho nào"
}
```

**Status Enum Values:**
- `0` = Pending (Chờ xử lý)
- `1` = Approved (Đã duyệt)
- `2` = Completed (Hoàn thành)
- `3` = Cancelled (Đã hủy)

---

## 2. Get Export Orders by Status

**Lấy danh sách đơn xuất theo trạng thái**

### Request
```http
GET /api/ExportOrder/by-status?status={statusEnum}
Accept: application/json
Authorization: Bearer {token}
```

**Query Parameters:**
- `status` (integer, required) - Trạng thái đơn xuất (0, 1, 2, 3)

**Examples:**
```http
GET /api/ExportOrder/by-status?status=0   # Pending
GET /api/ExportOrder/by-status?status=1   # Approved
GET /api/ExportOrder/by-status?status=2   # Completed
```

### Response Success (200 OK)
```json
[
  {
    "exportOrderId": 1,
    "invoiceNumber": "EXP-20251110-001",
    "orderDate": "2025-11-10",
    "customerId": 10,
    "currency": "VND",
    "createdDate": "2025-11-10",
    "shippedDate": "2025-11-12",
    "shippedAddress": "123 Đường Láng, Đống Đa, Hà Nội",
    "taxRate": 0.10,
    "taxAmount": 5000000,
    "totalPayment": 55000000,
    "description": "Đơn hàng xuất kho tháng 11",
    "status": 0,
    "createdBy": 1
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
  "message": "Không có đơn xuất kho nào với trạng thái Pending"
}
```

---

## 3. Create Export Order

**Tạo đơn xuất kho mới**

### Request
```http
POST /api/ExportOrder/ExportOder
Content-Type: application/json
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "invoiceNumber": "EXP-20251110-004",
  "orderDate": "2025-11-10",
  "customerId": 25,
  "currency": "VND",
  "shippedDate": "2025-11-12",
  "shippedAddress": "100 Hoàng Quốc Việt, Cầu Giấy, Hà Nội",
  "taxRate": 0.10,
  "taxAmount": 2000000,
  "totalPayment": 22000000,
  "description": "Đơn hàng mới từ khách hàng",
  "createdBy": 1
}
```

**Field Descriptions:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| invoiceNumber | string | ❌ No | Số hóa đơn (có thể auto-generate) |
| orderDate | string (DateOnly) | ✅ Yes | Ngày đặt hàng (yyyy-MM-dd) |
| customerId | integer | ✅ Yes | ID khách hàng |
| currency | string | ❌ No | Loại tiền tệ (VND, USD...) |
| shippedDate | string (DateOnly) | ❌ No | Ngày giao hàng dự kiến |
| shippedAddress | string | ❌ No | Địa chỉ giao hàng |
| taxRate | decimal | ❌ No | Thuế suất (0.10 = 10%) |
| taxAmount | decimal | ❌ No | Số tiền thuế |
| totalPayment | decimal | ❌ No | Tổng thanh toán |
| description | string | ❌ No | Mô tả đơn hàng |
| createdBy | integer | ❌ No | ID người tạo |

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "exportOrderId": 4,
    "invoiceNumber": "EXP-20251110-004",
    "orderDate": "2025-11-10",
    "customerId": 25,
    "currency": "VND",
    "createdDate": "2025-11-10",
    "shippedDate": "2025-11-12",
    "shippedAddress": "100 Hoàng Quốc Việt, Cầu Giấy, Hà Nội",
    "taxRate": 0.10,
    "taxAmount": 2000000,
    "totalPayment": 22000000,
    "description": "Đơn hàng mới từ khách hàng",
    "status": 0,
    "createdBy": 1
  },
  "message": "Tạo Export Order thành công"
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "CustomerId không tồn tại"
}
```

---

## 4. Update Export Order

**Cập nhật thông tin đơn xuất kho**

### Request
```http
PUT /api/ExportOrder/ExportOder?exportOrderId={id}
Content-Type: application/json
Authorization: Bearer {token}
```

**Query Parameters:**
- `exportOrderId` (integer, required) - ID của đơn xuất cần update

**Request Body:** (Tất cả fields đều optional)
```json
{
  "orderDate": "2025-11-11",
  "customerId": 25,
  "currency": "VND",
  "shippedDate": "2025-11-13",
  "shippedAddress": "100 Hoàng Quốc Việt, Cầu Giấy, Hà Nội (Cập nhật)",
  "taxRate": 0.10,
  "taxAmount": 2500000,
  "totalPayment": 27500000,
  "description": "Đơn hàng đã được cập nhật",
  "status": "Approved",
  "createdBy": 1
}
```

**Field Descriptions:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| orderDate | string (DateOnly) | ❌ No | Ngày đặt hàng mới |
| customerId | integer | ❌ No | ID khách hàng mới |
| currency | string | ❌ No | Loại tiền tệ mới |
| createdDate | string (DateOnly) | ❌ No | Ngày tạo mới |
| shippedDate | string (DateOnly) | ❌ No | Ngày giao hàng mới |
| shippedAddress | string | ❌ No | Địa chỉ giao hàng mới |
| taxRate | decimal | ❌ No | Thuế suất mới |
| taxAmount | decimal | ❌ No | Số tiền thuế mới |
| totalPayment | decimal | ❌ No | Tổng thanh toán mới |
| description | string | ❌ No | Mô tả mới |
| status | string | ❌ No | Trạng thái mới ("Pending", "Approved", "Completed") |
| createdBy | integer | ❌ No | ID người tạo |

**Example:**
```http
PUT /api/ExportOrder/ExportOder?exportOrderId=1
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "exportOrderId": 1,
    "invoiceNumber": "EXP-20251110-001",
    "orderDate": "2025-11-11",
    "customerId": 25,
    "currency": "VND",
    "createdDate": "2025-11-10",
    "shippedDate": "2025-11-13",
    "shippedAddress": "100 Hoàng Quốc Việt, Cầu Giấy, Hà Nội (Cập nhật)",
    "taxRate": 0.10,
    "taxAmount": 2500000,
    "totalPayment": 27500000,
    "description": "Đơn hàng đã được cập nhật",
    "status": 1,
    "createdBy": 1
  },
  "message": "Cập nhật Export Order thành công"
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Export Order không tồn tại"
}
```

**⚠️ Note:** Invoice Number không thể update (immutable)

---

## 5. Delete Export Order

**Xóa đơn xuất kho**

### Request
```http
DELETE /api/ExportOrder/ExportOder?exportOrderId={id}
Authorization: Bearer {token}
```

**Query Parameters:**
- `exportOrderId` (integer, required) - ID của đơn xuất cần xóa

**Example:**
```http
DELETE /api/ExportOrder/ExportOder?exportOrderId=4
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": null,
  "message": "Xóa Export Order thành công"
}
```

### Response Error (400 Bad Request) - Not Found
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Export Order không tồn tại"
}
```

### Response Error (400 Bad Request) - Has Dependencies
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Không thể xóa Export Order vì có Export Detail liên quan. Vui lòng xóa các details trước."
}
```

**⚠️ Important:**
- Phải xóa tất cả Export Details trước khi xóa Export Order
- Hoặc cascade delete (nếu được implement)
- Khi xóa cần hoàn trả tồn kho (restore inventory)

---

# EXPORT DETAILS (LINES)

## 6. Get Export Order Details

**Lấy danh sách chi tiết sản phẩm của đơn xuất**

### Request
```http
GET /api/ExportOrder/{exportOrderId}Details
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**
- `exportOrderId` (integer, required) - ID của đơn xuất

**Example:**
```http
GET /api/ExportOrder/1Details
```

### Response Success (200 OK)
```json
[
  {
    "exportDetailId": 1,
    "exportOrderId": 1,
    "productId": 100,
    "quantity": 10,
    "totalPrice": 250000000
  },
  {
    "exportDetailId": 2,
    "exportOrderId": 1,
    "productId": 101,
    "quantity": 20,
    "totalPrice": 50000000
  },
  {
    "exportDetailId": 3,
    "exportOrderId": 1,
    "productId": 102,
    "quantity": 15,
    "totalPrice": 48000000
  }
]
```

**Field Descriptions:**
| Field | Type | Description |
|-------|------|-------------|
| exportDetailId | integer | ID của chi tiết xuất |
| exportOrderId | integer | ID đơn xuất |
| productId | integer | ID sản phẩm |
| quantity | integer | Số lượng xuất |
| totalPrice | decimal | Tổng giá (quantity × unitPrice) |

### Response Error (404 Not Found)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 404,
  "data": null,
  "message": "Không tìm thấy Export Order hoặc chưa có chi tiết nào"
}
```

---

## 7. Create Export Detail

**Thêm sản phẩm vào đơn xuất**

### Request
```http
POST /api/ExportOrder/ExportDetail?exportOrderId={id}
Content-Type: application/json
Authorization: Bearer {token}
```

**Query Parameters:**
- `exportOrderId` (integer, required) - ID của đơn xuất

**Request Body:**
```json
{
  "productId": 103,
  "quantity": 5
}
```

**Field Descriptions:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| productId | integer | ✅ Yes | ID sản phẩm cần xuất |
| quantity | integer | ✅ Yes | Số lượng xuất (> 0) |

**Example:**
```http
POST /api/ExportOrder/ExportDetail?exportOrderId=1
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "exportDetailId": 4,
    "exportOrderId": 1,
    "productId": 103,
    "quantity": 5,
    "totalPrice": 12500000
  },
  "message": "Thêm Export Detail thành công"
}
```

### Response Error (400 Bad Request) - Invalid Export Order
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Export Order không tồn tại"
}
```

### Response Error (400 Bad Request) - Product Not Found
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Sản phẩm không tồn tại"
}
```

### Response Error (400 Bad Request) - Insufficient Stock
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Không đủ tồn kho. Hiện có: 3, Yêu cầu: 5"
}
```

### Response Error (400 Bad Request) - Product Expired
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Sản phẩm đã hết hạn, không thể xuất kho"
}
```

**Business Rules:**
1. ✅ Export Order phải tồn tại
2. ✅ Product phải tồn tại
3. ✅ Quantity > 0
4. ✅ Tồn kho đủ (QuantityInStock >= quantity)
5. ✅ Sản phẩm chưa hết hạn (ExpiredDate > Today)
6. ✅ TotalPrice = Quantity × Product.UnitPrice
7. ✅ Giảm tồn kho sau khi tạo detail

---

## 8. Update Export Detail

**Cập nhật chi tiết sản phẩm trong đơn xuất**

### Request
```http
PUT /api/ExportOrder/ExportDetail?exportDetailId={id}
Content-Type: application/json
Authorization: Bearer {token}
```

**Query Parameters:**
- `exportDetailId` (integer, required) - ID của chi tiết xuất cần update

**Request Body:** (Tất cả fields đều optional)
```json
{
  "productId": 100,
  "quantity": 15
}
```

**Field Descriptions:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| productId | integer | ❌ No | ID sản phẩm mới |
| quantity | integer | ❌ No | Số lượng mới |

**Example:**
```http
PUT /api/ExportOrder/ExportDetail?exportDetailId=1
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": {
    "exportDetailId": 1,
    "exportOrderId": 1,
    "productId": 100,
    "quantity": 15,
    "totalPrice": 375000000
  },
  "message": "Cập nhật Export Detail thành công"
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Export Detail không tồn tại"
}
```

**Business Logic khi update quantity:**
```
Ví dụ: Update từ 10 → 15
1. Hoàn trả tồn kho cũ: QuantityInStock += 10
2. Validate tồn kho mới: QuantityInStock >= 15
3. Trừ tồn kho mới: QuantityInStock -= 15
4. Update totalPrice = 15 × unitPrice
```

---

## 9. Delete Export Detail

**Xóa chi tiết sản phẩm khỏi đơn xuất**

### Request
```http
DELETE /api/ExportOrder/ExportDetail?exportDetailId={id}
Authorization: Bearer {token}
```

**Query Parameters:**
- `exportDetailId` (integer, required) - ID của chi tiết xuất cần xóa

**Example:**
```http
DELETE /api/ExportOrder/ExportDetail?exportDetailId=4
```

### Response Success (200 OK)
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": null,
  "message": "Xóa Export Detail thành công"
}
```

### Response Error (400 Bad Request)
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400,
  "data": null,
  "message": "Export Detail không tồn tại"
}
```

**⚠️ Important:**
- Khi xóa Export Detail, phải HOÀN TRẢ tồn kho
- `QuantityInStock += deletedQuantity`
- Update lại `totalPayment` của Export Order

---

## 📊 Common Response Structure

### Success Response
```json
{
  "isSuccess": true,
  "responseCode": null,
  "statusCode": 200,
  "data": { /* ExportOrderResponse or ExportDetailResponse */ },
  "message": "Success message in Vietnamese"
}
```

### Error Response
```json
{
  "isSuccess": false,
  "responseCode": null,
  "statusCode": 400 | 404,
  "data": null,
  "message": "Error message in Vietnamese"
}
```

---

## 🔄 Workflow: Tạo đơn xuất hoàn chỉnh

### Step 1: Tạo Export Order (Header)
```http
POST /api/ExportOrder/ExportOder
{
  "orderDate": "2025-11-10",
  "customerId": 25,
  "currency": "VND",
  "shippedDate": "2025-11-12",
  "shippedAddress": "123 ABC, Hà Nội",
  "createdBy": 1
}

→ Response: { "exportOrderId": 5 }
```

### Step 2: Thêm sản phẩm (Details)
```http
POST /api/ExportOrder/ExportDetail?exportOrderId=5
{ "productId": 100, "quantity": 10 }

POST /api/ExportOrder/ExportDetail?exportOrderId=5
{ "productId": 101, "quantity": 20 }

POST /api/ExportOrder/ExportDetail?exportOrderId=5
{ "productId": 102, "quantity": 15 }
```

### Step 3: Tính tổng và update Order
```http
PUT /api/ExportOrder/ExportOder?exportOrderId=5
{
  "taxRate": 0.10,
  "taxAmount": 34800000,
  "totalPayment": 382800000
}
```

### Step 4: Approve đơn hàng
```http
PUT /api/ExportOrder/ExportOder?exportOrderId=5
{
  "status": "Approved"
}
```

### Step 5: Hoàn thành đơn hàng
```http
PUT /api/ExportOrder/ExportOder?exportOrderId=5
{
  "status": "Completed"
}
```

---

## 🎯 Business Rules Summary

### Export Order Rules:
1. **Create:**
   - CustomerId phải tồn tại
   - OrderDate không được trong quá khứ xa
   - Currency mặc định là "VND"
   - Status mặc định là Pending (0)

2. **Update:**
   - Không update InvoiceNumber (immutable)
   - Có thể update status: Pending → Approved → Completed
   - Không nên update khi status = Completed

3. **Delete:**
   - Kiểm tra Export Details trước khi xóa
   - Hoàn trả tồn kho nếu có details
   - Không nên xóa đơn đã Completed

### Export Detail Rules:
1. **Create:**
   - Validate tồn kho đủ
   - Validate sản phẩm chưa hết hạn
   - Auto calculate totalPrice
   - Giảm tồn kho ngay khi tạo

2. **Update:**
   - Hoàn trả tồn kho cũ
   - Validate tồn kho mới
   - Trừ tồn kho mới
   - Update totalPrice

3. **Delete:**
   - Hoàn trả tồn kho
   - Update totalPayment của Order

---

## 🔒 Security Recommendations

⚠️ **Current State:** Không có authorization

**Khuyến nghị:**
```csharp
[Authorize] // Yêu cầu đăng nhập
public class ExportOrderController : BaseApiController
{
    [HttpGet("All")]
    public async Task<IActionResult> GetAllExportOrder() { }
    
    [HttpPost("ExportOder")]
    [Authorize(Roles = "1,2")] // Staff và Manager
    public async Task<IActionResult> CreateExportOrder() { }
    
    [HttpPut("ExportOder")]
    [Authorize(Roles = "1,2")]
    public async Task<IActionResult> UpdateExportOrder() { }
    
    [HttpDelete("ExportOder")]
    [Authorize(Roles = "1")] // Chỉ Manager
    public async Task<IActionResult> DeleteExportOrder() { }
    
    [HttpPost("ExportDetail")]
    [Authorize(Roles = "1,2")]
    public async Task<IActionResult> CreateExportDetail() { }
}
```

---

## ⚠️ Important Notes

### 1. Inventory Management
```
Khi Create Detail: QuantityInStock -= quantity
Khi Update Detail: 
  - Restore old: QuantityInStock += oldQuantity
  - Deduct new: QuantityInStock -= newQuantity
Khi Delete Detail: QuantityInStock += quantity
```

### 2. Price Calculation
```
TotalPrice (Detail) = Quantity × Product.UnitPrice
TotalPayment (Order) = Sum(Details.TotalPrice) + TaxAmount
TaxAmount = Sum(Details.TotalPrice) × TaxRate
```

### 3. Status Workflow
```
Pending (0) → Approved (1) → Completed (2)
           ↘ Cancelled (3)
```

### 4. Validation Checklist
- [ ] Export Order exists
- [ ] Product exists
- [ ] Product not expired
- [ ] Quantity > 0
- [ ] Sufficient stock
- [ ] Customer exists
- [ ] Status transition valid

---

## 📝 Testing Checklist

**Export Orders:**
- [ ] GET all export orders
- [ ] GET by status (Pending, Approved, Completed)
- [ ] POST create export order (success)
- [ ] POST create with invalid customer (400)
- [ ] PUT update export order (success)
- [ ] PUT update non-existent order (400)
- [ ] DELETE export order without details (success)
- [ ] DELETE export order with details (400)

**Export Details:**
- [ ] GET export order details (success)
- [ ] GET details of non-existent order (404)
- [ ] POST create detail (success)
- [ ] POST create with insufficient stock (400)
- [ ] POST create with expired product (400)
- [ ] PUT update detail quantity (success)
- [ ] PUT update detail product (success)
- [ ] DELETE detail (success)
- [ ] Verify inventory restored after delete

**Integration Tests:**
- [ ] Complete workflow: Create Order → Add Details → Update → Complete
- [ ] Verify inventory decreased correctly
- [ ] Verify totalPrice calculated correctly
- [ ] Verify cannot export expired products
- [ ] Verify cannot exceed available stock

---

## 🐛 Known Issues & Improvements

### Current Issues:
1. ❌ Typo in endpoint: `ExportOder` → should be `ExportOrder`
2. ❌ No pagination for GetAll
3. ❌ No authorization
4. ❌ No date range filter
5. ❌ No search functionality

### Suggested Improvements:
```csharp
// Better endpoint naming
[HttpPost("orders")] // instead of ExportOder
[HttpPost("orders/{orderId}/details")] // RESTful design

// Add filters
[HttpGet("orders")]
public async Task<IActionResult> GetOrders(
    [FromQuery] DateOnly? from,
    [FromQuery] DateOnly? to,
    [FromQuery] int? customerId,
    [FromQuery] StatusEnums? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20
)

// Add search
[HttpGet("orders/search")]
public async Task<IActionResult> SearchOrders(
    [FromQuery] string q
)
```

