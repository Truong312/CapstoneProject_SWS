# Tóm tắt mối quan hệ giữa Product, Import Orders và Export Orders API

## 📋 Tổng quan 3 API

### 1. **Product API** (`/api/product`)
Quản lý thông tin sản phẩm trong kho

**Endpoints chính:**
- `GET /api/product` - Lấy tất cả sản phẩm
- `GET /api/product/{id}` - Chi tiết sản phẩm
- `POST /api/product` - Tạo sản phẩm mới
- `PUT /api/product/{id}` - Cập nhật sản phẩm
- `DELETE /api/product/{id}` - Xóa sản phẩm
- `GET /api/product/near-expired` - Sản phẩm sắp hết hạn (<30 ngày)
- `GET /api/product/expired` - Sản phẩm đã hết hạn
- `GET /api/product/low-stock` - Sản phẩm tồn kho thấp
- `GET /api/product/search?text=abc` - Tìm kiếm sản phẩm
- `GET /api/product/paged?page=1&pageSize=20` - Phân trang

**Thông tin sản phẩm:**
```json
{
  "productId": 100,
  "serialNumber": "PROD-001",
  "name": "Laptop Dell XPS 15",
  "expiredDate": "2026-12-31",
  "unit": "Cái",
  "unitPrice": 25000000,
  "receivedDate": "2025-11-10",
  "purchasedPrice": 20000000,
  "reorderPoint": 5,
  "image": "url/to/image",
  "description": "Mô tả sản phẩm"
}
```

---

### 2. **Import Orders API** (`/api/import-orders`)
Quản lý đơn nhập hàng vào kho

**Endpoints chính:**
- `GET /api/import-orders` - Danh sách đơn nhập (filter + paging)
  - Query params: `q`, `providerId`, `status`, `from`, `to`, `page`, `pageSize`
- `GET /api/import-orders/{id}` - Chi tiết đơn nhập
- `POST /api/import-orders` - Tạo đơn nhập mới *(Staff role=1)*

**Cấu trúc đơn nhập:**
```json
{
  "importOrderId": 1,
  "invoiceNumber": "IMP-20251110-001",
  "orderDate": "2025-11-10",
  "providerId": 5,
  "providerName": "Công ty ABC",
  "status": "Pending",
  "createdBy": 1,
  "createdByName": "Nguyễn Văn A",
  "items": [
    {
      "importDetailId": 1,
      "productId": 100,
      "productName": "Laptop Dell XPS 15",
      "quantity": 10,
      "importPrice": 20000000
    }
  ]
}
```

**Tạo đơn nhập mới:**
```json
POST /api/import-orders
{
  "providerId": 5,
  "orderDate": "2025-11-10",
  "invoiceNumber": "IMP-20251110-004",
  "items": [
    {
      "productId": 100,
      "quantity": 10,
      "importPrice": 20000000
    }
  ]
}
```

---

### 3. **Export Orders API** (`/api/ExportOrder`)
Quản lý đơn xuất hàng ra khỏi kho

**Endpoints chính:**
- `GET /api/ExportOrder/All` - Tất cả đơn xuất
- `GET /api/ExportOrder/by-status?status=0` - Lọc theo trạng thái (0=Pending, 1=Approved, 2=Completed)
- `GET /api/ExportOrder/{id}Details` - Chi tiết đơn xuất
- `POST /api/ExportOrder/ExportOder` - Tạo đơn xuất
- `POST /api/ExportOrder/ExportDetail?exportOrderId=1` - Thêm sản phẩm vào đơn xuất
- `PUT /api/ExportOrder/ExportOder?exportOrderId=1` - Cập nhật đơn xuất
- `PUT /api/ExportOrder/ExportDetail?exportDetailId=1` - Cập nhật chi tiết
- `DELETE /api/ExportOrder/ExportOder?exportOrderId=1` - Xóa đơn xuất
- `DELETE /api/ExportOrder/ExportDetail?exportDetailId=1` - Xóa chi tiết

**Cấu trúc đơn xuất:**
```json
{
  "exportOrderId": 1,
  "invoiceNumber": "EXP-20251110-001",
  "orderDate": "2025-11-10",
  "customerId": 10,
  "currency": "VND",
  "shippedDate": "2025-11-12",
  "shippedAddress": "123 Đường Láng, Hà Nội",
  "taxRate": 0.10,
  "taxAmount": 5000000,
  "totalPayment": 55000000,
  "status": 0,
  "createdBy": 1
}
```

**Chi tiết đơn xuất:**
```json
[
  {
    "exportDetailId": 1,
    "exportOrderId": 1,
    "productId": 100,
    "quantity": 10,
    "totalPrice": 250000000
  }
]
```

---

## 🔄 Mối quan hệ và ảnh hưởng khi thay đổi sản phẩm

### **Luồng dữ liệu:**

```
┌─────────────────┐
│   PRODUCT API   │  ← Master data (sản phẩm)
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
    ▼         ▼
┌────────┐ ┌────────┐
│ IMPORT │ │ EXPORT │  ← Transaction data (giao dịch)
└────────┘ └────────┘
```

### **1. Khi TẠO sản phẩm mới (POST /api/product)**

**Ảnh hưởng:**
- ✅ **Import Orders**: Có thể tạo đơn nhập cho sản phẩm mới
- ✅ **Export Orders**: Có thể tạo đơn xuất cho sản phẩm mới
- 📝 **Lưu ý**: Nên tạo sản phẩm trước khi tạo đơn nhập/xuất

**Ví dụ luồng:**
```
1. POST /api/product → Tạo sản phẩm (productId=100)
2. POST /api/import-orders → Nhập 50 cái vào kho
3. POST /api/ExportOrder/ExportOder → Xuất 20 cái ra
```

---

### **2. Khi CẬP NHẬT sản phẩm (PUT /api/product/{id})**

**Các thay đổi có thể ảnh hưởng:**

#### a) **Thay đổi giá bán (UnitPrice)**
- ❌ **Không ảnh hưởng** đến đơn nhập/xuất đã tạo (dữ liệu lịch sử)
- ✅ **Ảnh hưởng** đến đơn xuất mới tạo sau này
- 💡 **Recommendation**: Nên có history tracking cho giá

```json
// Cập nhật giá sản phẩm
PUT /api/product/100
{
  "unitPrice": 27000000  // Tăng từ 25tr → 27tr
}

// Đơn xuất CŨ: Vẫn giữ giá cũ (tính theo thời điểm xuất)
// Đơn xuất MỚI: Sẽ tính theo giá mới 27tr
```

#### b) **Thay đổi tên sản phẩm (Name)**
- ⚠️ **Có thể ảnh hưởng** nếu hệ thống join realtime
- 🔍 **Check code**: Xem có cache hay không
- 📊 **Report**: Cần update để hiển thị tên mới

```json
PUT /api/product/100
{
  "name": "Laptop Dell XPS 15 Gen 2024"  // Đổi tên
}

// Import/Export detail có thể hiển thị tên mới nếu join từ Product table
```

#### c) **Thay đổi ReorderPoint (Ngưỡng đặt hàng lại)**
- ✅ **Ảnh hưởng** đến endpoint `/api/product/low-stock`
- 🔔 **Alert**: Có thể trigger notification cho staff

```json
PUT /api/product/100
{
  "reorderPoint": 10  // Tăng từ 5 → 10
}

// Nếu tồn kho = 8:
// - Trước: Không low-stock (8 > 5)
// - Sau: Là low-stock (8 < 10)
```

#### d) **Thay đổi ExpiredDate (Hạn sử dụng)**
- ✅ **Ảnh hưởng** đến:
  - `/api/product/near-expired` (sản phẩm sắp hết hạn)
  - `/api/product/expired` (sản phẩm đã hết hạn)
- 🚫 **Business rule**: Không nên xuất sản phẩm hết hạn

---

### **3. Khi XÓA sản phẩm (DELETE /api/product/{id})**

**⚠️ Rủi ro cao - Cần validation:**

#### **Trường hợp 1: Sản phẩm có Import/Export Details**
```
❌ KHÔNG NÊN XÓA
Lý do: 
- Đơn nhập cũ sẽ bị mất reference (productId không tồn tại)
- Đơn xuất cũ sẽ bị mất reference
- Báo cáo sẽ bị sai
```

**Recommendation:**
```csharp
// Nên check trước khi xóa:
DELETE /api/product/100

// Backend nên validate:
if (HasImportDetails(productId) || HasExportDetails(productId))
{
    return BadRequest("Không thể xóa sản phẩm đã có giao dịch. Hãy deactivate thay vì xóa.");
}
```

#### **Trường hợp 2: Sản phẩm chưa có giao dịch**
```
✅ CÓ THỂ XÓA
Nhưng nên:
- Soft delete (isActive = false)
- Thay vì hard delete
```

---

### **4. Khi TẠO Import Order (POST /api/import-orders)**

**Ảnh hưởng đến Product:**

```json
POST /api/import-orders
{
  "items": [
    { "productId": 100, "quantity": 10, "importPrice": 20000000 }
  ]
}
```

**Các thay đổi:**
1. ✅ **Tồn kho tăng**: `QuantityInStock += 10`
2. ✅ **Có thể cập nhật**: `PurchasedPrice = 20000000` (giá mua mới nhất)
3. 🔔 **Alert**: Nếu sản phẩm đang low-stock → Không còn low-stock

**Validation cần thiết:**
- ✔️ Product phải tồn tại
- ✔️ Quantity > 0
- ✔️ ImportPrice >= 0

---

### **5. Khi TẠO Export Order (POST /api/ExportOrder)**

**Ảnh hưởng đến Product:**

```json
POST /api/ExportOrder/ExportDetail?exportOrderId=1
{
  "productId": 100,
  "quantity": 5
}
```

**Các thay đổi:**
1. ✅ **Tồn kho giảm**: `QuantityInStock -= 5`
2. 🔔 **Alert**: Nếu tồn kho sau xuất < ReorderPoint → Trigger low-stock warning
3. 📊 **Revenue**: Tính doanh thu = `quantity * unitPrice`

**Validation cần thiết:**
- ✔️ Product phải tồn tại
- ✔️ Quantity > 0
- ✔️ **Tồn kho đủ**: `QuantityInStock >= quantity`
- ✔️ **Chưa hết hạn**: `ExpiredDate > Today`

---

## 🎯 Các trường hợp cần xử lý đặc biệt

### **Case 1: Xóa Import/Export Detail**

```
DELETE /api/ExportOrder/ExportDetail?exportDetailId=1

→ Phải HOÀN TRẢ tồn kho:
  QuantityInStock += deletedQuantity
```

```
DELETE /api/import-orders/{id} (nếu có endpoint)

→ Phải TRỪ tồn kho:
  QuantityInStock -= importedQuantity
```

### **Case 2: Update quantity trong Import/Export Detail**

```
PUT /api/ExportOrder/ExportDetail?exportDetailId=1
{
  "quantity": 15  // Tăng từ 10 → 15
}

→ Phải điều chỉnh tồn kho:
  QuantityInStock -= (newQuantity - oldQuantity)
  QuantityInStock -= (15 - 10) = -5
```

### **Case 3: Sản phẩm hết hạn nhưng vẫn trong kho**

```
GET /api/product/expired

→ Hiển thị sản phẩm có ExpiredDate < Today
→ Cần xử lý:
  1. Tạo Return Order (nếu có API)
  2. Hoặc tạo Adjustment Order để loại bỏ
  3. Không cho phép xuất kho
```

---

## 📊 Tóm tắt mối quan hệ

| Thao tác | Ảnh hưởng Product | Ảnh hưởng Import | Ảnh hưởng Export |
|----------|-------------------|------------------|------------------|
| **Tạo Product** | Thêm sản phẩm mới | Có thể nhập | Có thể xuất |
| **Xóa Product** | ⚠️ Cần check references | ❌ Bị mất reference | ❌ Bị mất reference |
| **Update Product Price** | Giá mới | ❌ Không ảnh hưởng | ✅ Đơn mới dùng giá mới |
| **Update Reorder Point** | Thay đổi ngưỡng | - | - |
| **Tạo Import Order** | ✅ Tăng tồn kho | Thêm lịch sử nhập | - |
| **Tạo Export Order** | ✅ Giảm tồn kho | - | Thêm lịch sử xuất |
| **Xóa Import Detail** | ✅ Giảm tồn kho | Xóa lịch sử | - |
| **Xóa Export Detail** | ✅ Tăng tồn kho | - | Xóa lịch sử |

---

## 💡 Khuyến nghị cho Frontend

### **1. Validation khi tạo Export Order:**
```typescript
// Check tồn kho trước khi submit
if (product.quantityInStock < requestQuantity) {
  alert("Không đủ hàng trong kho!");
}

// Check hạn sử dụng
if (product.expiredDate < today) {
  alert("Sản phẩm đã hết hạn, không thể xuất!");
}
```

### **2. Real-time update sau khi Import/Export:**
```typescript
// Sau khi tạo Import Order thành công
await createImportOrder(data);
await refreshProductList(); // Cập nhật lại tồn kho

// Hiển thị notification nếu low-stock được giải quyết
if (wasLowStock && !isLowStockNow) {
  showSuccess("Đã bổ sung tồn kho!");
}
```

### **3. Confirmation trước khi xóa Product:**
```typescript
async function deleteProduct(productId) {
  const hasTransactions = await checkProductTransactions(productId);
  
  if (hasTransactions) {
    showError("Sản phẩm đã có giao dịch, không thể xóa!");
    return;
  }
  
  if (confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
    await api.delete(`/api/product/${productId}`);
  }
}
```

---

## 🔐 Authorization Summary

| Endpoint | Required Role | Note |
|----------|---------------|------|
| `POST /api/import-orders` | Staff (role=1) | Chỉ staff mới tạo được đơn nhập |
| `GET /api/import-orders` | Any authenticated | Cần đăng nhập |
| `GET /api/import-orders/{id}` | Any authenticated | Cần đăng nhập |
| `POST /api/product` | ? | Chưa có [Authorize] |
| `POST /api/ExportOrder/*` | ? | Chưa có [Authorize] |

**⚠️ Lưu ý**: Export Order và Product Controller chưa có authorization, cần bổ sung!

---

## 📝 Kết luận

**Mối quan hệ chính:**
- **Product** = Master data (dữ liệu gốc)
- **Import Orders** = Giao dịch nhập → Tăng tồn kho
- **Export Orders** = Giao dịch xuất → Giảm tồn kho

**Nguyên tắc quan trọng:**
1. ✅ Không xóa Product đã có giao dịch
2. ✅ Luôn validate tồn kho trước khi xuất
3. ✅ Cập nhật tồn kho realtime khi Import/Export
4. ✅ Không xuất sản phẩm hết hạn
5. ✅ Track lịch sử giá để báo cáo chính xác

