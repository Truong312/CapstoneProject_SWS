# Tài liệu API cho Nghiệp Vụ Kho (Warehouse Operations)

Đây là tài liệu mô tả các endpoint liên quan đến nghiệp vụ kho, bao gồm quản lý đơn nhập hàng và các API phụ trợ.

---

## 1. Lấy danh sách đơn nhập hàng (Filter & Paging)

Lấy danh sách các đơn nhập hàng có hỗ trợ tìm kiếm, lọc theo nhiều tiêu chí và phân trang.

- **Endpoint**: `GET /api/import-orders`
- **Quyền yêu cầu**: Cần đăng nhập (`[Authorize]`)

### Request

#### Query Parameters

| Tên | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `q` | string | Không | Tìm kiếm theo số hóa đơn (`InvoiceNumber`). |
| `providerId` | int | Không | Lọc theo ID của nhà cung cấp. |
| `status` | string | Không | Lọc theo trạng thái đơn hàng (ví dụ: "Pending", "Completed", "Canceled"). |
| `from` | DateOnly | Không | Lọc theo ngày bắt đầu (định dạng `YYYY-MM-DD`). |
| `to` | DateOnly | Không | Lọc theo ngày kết thúc (định dạng `YYYY-MM-DD`). |
| `page` | int | Không | Số trang hiện tại (mặc định là `1`). |
| `pageSize` | int | Không | Số lượng mục trên mỗi trang (mặc định là `20`). |

#### Ví dụ Request
```http
GET /api/import-orders?status=Pending&providerId=1&page=1&pageSize=10
```

### Response

#### Thành công (200 OK)
```json
{
    "total": 15,
    "page": 1,
    "pageSize": 10,
    "items": [
        {
            "importOrderId": 101,
            "invoiceNumber": "PN-2024-00101",
            "orderDate": "2024-10-20",
            "providerName": "Nhà Cung Cấp A",
            "status": "Pending",
            "totalItems": 5,
            "createdByName": "Nguyen Van A"
        },
        {
            "importOrderId": 102,
            "invoiceNumber": "PN-2024-00102",
            "orderDate": "2024-10-21",
            "providerName": "Nhà Cung Cấp B",
            "status": "Pending",
            "totalItems": 3,
            "createdByName": "Tran Thi B"
        }
    ]
}
```

---

## 2. Lấy chi tiết đơn nhập hàng

Lấy thông tin chi tiết của một đơn nhập hàng dựa vào ID.

- **Endpoint**: `GET /api/import-orders/{id}`
- **Quyền yêu cầu**: Cần đăng nhập (`[Authorize]`)

### Request

#### Route Parameter
| Tên | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `id` | int | Có | ID của đơn nhập hàng cần xem chi tiết. |

#### Ví dụ Request
```http
GET /api/import-orders/101
```

### Response

#### Thành công (200 OK)
```json
{
    "importOrderId": 101,
    "invoiceNumber": "PN-2024-00101",
    "orderDate": "2024-10-20",
    "providerId": 1,
    "providerName": "Nhà Cung Cấp A",
    "status": "Pending",
    "createdDate": "2024-10-20",
    "createdBy": 12,
    "createdByName": "Nguyen Van A",
    "items": [
        {
            "importDetailId": 201,
            "productId": 1,
            "productName": "Sản phẩm X",
            "quantity": 100,
            "importPrice": 150000.0
        },
        {
            "importDetailId": 202,
            "productId": 2,
            "productName": "Sản phẩm Y",
            "quantity": 50,
            "importPrice": 250000.0
        }
    ]
}
```

#### Thất bại (404 Not Found)
Khi không tìm thấy đơn nhập hàng với ID tương ứng.

---

## 3. Tạo đơn nhập hàng

Tạo một đơn nhập hàng mới.

- **Endpoint**: `POST /api/import-orders`
- **Quyền yêu cầu**: Nhân viên (`[Authorize(Roles = "1")]`) - *hiện tại đang được comment lại*

### Request

#### Body
```json
{
    "providerId": 1,
    "orderDate": "2024-10-20",
    "invoiceNumber": "HD-NCC-001", // Có thể để null để hệ thống tự sinh
    "items": [
        {
            "productId": 1,
            "quantity": 100,
            "importPrice": 150000.0
        },
        {
            "productId": 2,
            "quantity": 50,
            "importPrice": 250000.0
        }
    ]
}
```

### Response

#### Thành công (201 Created)
```json
{
    "importOrderId": 101,
    "invoiceNumber": "PN-2024-00101"
}
```
- **Header `Location`**: Chứa URL để truy cập chi tiết đơn hàng vừa tạo (ví dụ: `/api/import-orders/101`).

#### Thất bại (400 Bad Request)
Khi dữ liệu không hợp lệ (ví dụ: `items` rỗng, `quantity` <= 0).
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "Items": [
            "Danh sách sản phẩm phải có ít nhất 1 dòng."
        ]
    }
}
```

---

## 4. Duyệt đơn nhập hàng

Duyệt hoặc từ chối một đơn nhập hàng đang ở trạng thái "Pending".

- **Endpoint**: `PUT /api/import-orders/{id}/review`
- **Quyền yêu cầu**: Quản lý (`[Authorize(Roles = "2")]`)

### Request

#### Route Parameter
| Tên | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `id` | int | Có | ID của đơn nhập hàng cần duyệt. |

#### Body
```json
{
    "approve": true,
    "notes": "Đã kiểm tra và xác nhận đủ hàng." // Ghi chú (tùy chọn)
}
```
- `approve = true`: Duyệt đơn hàng. Trạng thái sẽ chuyển thành `Completed` và tồn kho được cập nhật.
- `approve = false`: Từ chối/Hủy đơn hàng. Trạng thái sẽ chuyển thành `Canceled`.

### Response

#### Thành công (200 OK)
```json
{
    "isSuccess": true,
    "statusCode": 200,
    "message": "Đơn nhập hàng đã được duyệt thành công.",
    "data": null
}
```

#### Thất bại (ví dụ: 400 Bad Request)
Khi đơn hàng không ở trạng thái "Pending" hoặc có lỗi nghiệp vụ khác.
```json
{
    "isSuccess": false,
    "statusCode": 400,
    "message": "Chỉ có thể duyệt đơn hàng ở trạng thái 'Pending'.",
    "data": null
}
```

---

## 5. Lấy danh sách nhà cung cấp (Providers)

API này dùng để lấy danh sách tất cả các nhà cung cấp, phục vụ cho chức năng lọc hoặc tạo đơn nhập hàng.

- **Endpoint**: `GET /api/business-partners/providers`
- **Quyền yêu cầu**: Cần đăng nhập (`[Authorize]`)

### Request

#### Ví dụ Request
```http
GET /api/business-partners/providers
```

### Response

#### Thành công (200 OK)
```json
[
    {
        "partnerId": 1,
        "name": "Nhà Cung Cấp A"
    },
    {
        "partnerId": 2,
        "name": "Nhà Cung Cấp B"
    },
    {
        "partnerId": 5,
        "name": "Công ty TNHH Vật Tư Kho Bãi"
    }
]
```
