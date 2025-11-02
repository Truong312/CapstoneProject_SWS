using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;

namespace SWS.BusinessObjects.Extensions
{
    public static class DataSeedingExtension
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Seed Users (Admin, Quản lý, Nhân viên)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Nguyễn Văn Admin",
                    Email = "admin@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Phone = "0901234567",
                    Address = "123 Đường Láng, Đống Đa, Hà Nội",
                    Role = 1 // Admin
                },
                new User
                {
                    UserId = 2,
                    FullName = "Trần Thị Lan",
                    Email = "quanly@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("QuanLy@123"),
                    Phone = "0902345678",
                    Address = "456 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                    Role = 2 // Manager
                },
                new User
                {
                    UserId = 3,
                    FullName = "Lê Văn Hùng",
                    Email = "nhanvien1@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0903456789",
                    Address = "789 Giải Phóng, Hai Bà Trưng, Hà Nội",
                    Role = 3 // Staff
                },
                new User
                {
                    UserId = 4,
                    FullName = "Phạm Thị Mai",
                    Email = "nhanvien2@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0904567890",
                    Address = "321 Cầu Giấy, Hà Nội",
                    Role = 3 // Staff
                },
                new User
                {
                    UserId = 5,
                    FullName = "Hoàng Minh Tuấn",
                    Email = "nhanvien3@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0905678901",
                    Address = "100 Tây Sơn, Đống Đa, Hà Nội",
                    Role = 3 // Staff
                }
            );

            // Seed Business Partners (Nhà cung cấp & Khách hàng)
            modelBuilder.Entity<BusinessPartner>().HasData(
                new BusinessPartner
                {
                    PartnerId = 1,
                    Name = "Công ty TNHH Điện Tử Việt Nam",
                    Email = "contact@dientuVN.com",
                    Phone = "0241234567",
                    Address = "100 Khu Công Nghệ Cao, Quận 9, TP.HCM",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    PartnerId = 2,
                    Name = "Tập Đoàn Công Nghệ FPT",
                    Email = "sales@fpt.com.vn",
                    Phone = "0242345678",
                    Address = "200 Lê Văn Lương, Thanh Xuân, Hà Nội",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    PartnerId = 3,
                    Name = "Nhà Phân Phối Thế Giới Di Động",
                    Email = "orders@thegioididong.com",
                    Phone = "0243456789",
                    Address = "300 Trần Hưng Đạo, Quận 1, TP.HCM",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    PartnerId = 4,
                    Name = "Siêu Thị Điện Máy Xanh",
                    Email = "procurement@dienmayxanh.com",
                    Phone = "0244567890",
                    Address = "400 Nguyễn Văn Linh, Quận 7, TP.HCM",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    PartnerId = 5,
                    Name = "Công ty CP Phân Phối Hàng Hóa Việt",
                    Email = "info@phanphoiviet.com",
                    Phone = "0245678901",
                    Address = "500 Hải Phòng, Đà Nẵng",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    PartnerId = 6,
                    Name = "Nhà Cung Cấp Linh Kiện Điện Tử ABC",
                    Email = "sales@linhkienabc.com",
                    Phone = "0246789012",
                    Address = "600 Khu Công Nghiệp Tân Thuận, TP.HCM",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    PartnerId = 7,
                    Name = "Chuỗi Siêu Thị VinMart",
                    Email = "order@vinmart.com.vn",
                    Phone = "0247890123",
                    Address = "700 Phạm Văn Đồng, Bắc Từ Liêm, Hà Nội",
                    Type = "Customer"
                }
            );

            // Seed Locations (Kho hàng với vị trí cụ thể)
            var locations = new List<Location>();
            int locationId = 1;

            // Tạo kho: 5 kệ (A-E), mỗi kệ 6 cột x 6 tầng = 180 vị trí
            string[] shelves = { "A", "B", "C", "D", "E" };
            foreach (var shelf in shelves)
            {
                for (int col = 1; col <= 6; col++)
                {
                    for (int row = 1; row <= 6; row++)
                    {
                        locations.Add(new Location
                        {
                            LocationId = locationId++,
                            ShelfId = shelf,
                            ColumnNumber = col,
                            RowNumber = row,
                            Type = row <= 2 ? "Tầng Thấp" : (row <= 4 ? "Tầng Trung" : "Tầng Cao"),
                            IsFull = false
                        });
                    }
                }
            }

            modelBuilder.Entity<Location>().HasData(locations);

            // Seed Products (Nhiều sản phẩm đa dạng bằng tiếng Việt)
            modelBuilder.Entity<Product>().HasData(
                // Điện thoại & Tablet
                new Product
                {
                    ProductId = 1,
                    SerialNumber = "DT-IP14-001",
                    Name = "Điện thoại iPhone 14 Pro Max 256GB",
                    ExpiredDate = new DateOnly(2026, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 32000000m,
                    ReceivedDate = new DateOnly(2024, 1, 15),
                    PurchasedPrice = 28000000m,
                    ReorderPoint = 10,
                    Image = "https://res.cloudinary.com/demo/iphone14.jpg",
                    Description = "Điện thoại cao cấp Apple, chip A16 Bionic, camera 48MP"
                },
                new Product
                {
                    ProductId = 2,
                    SerialNumber = "DT-SS-002",
                    Name = "Samsung Galaxy S23 Ultra 512GB",
                    ExpiredDate = new DateOnly(2026, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 28000000m,
                    ReceivedDate = new DateOnly(2024, 2, 1),
                    PurchasedPrice = 24000000m,
                    ReorderPoint = 15,
                    Image = "https://res.cloudinary.com/demo/samsung-s23.jpg",
                    Description = "Smartphone flagship Samsung, màn hình Dynamic AMOLED 2X"
                },
                new Product
                {
                    ProductId = 3,
                    SerialNumber = "TB-IPAD-003",
                    Name = "iPad Pro M2 12.9 inch 256GB WiFi",
                    ExpiredDate = new DateOnly(2027, 6, 30),
                    Unit = "Cái",
                    UnitPrice = 30000000m,
                    ReceivedDate = new DateOnly(2024, 2, 10),
                    PurchasedPrice = 26000000m,
                    ReorderPoint = 8,
                    Image = "https://res.cloudinary.com/demo/ipad-pro.jpg",
                    Description = "Máy tính bảng chuyên nghiệp, chip M2, màn hình Liquid Retina XDR"
                },
                new Product
                {
                    ProductId = 4,
                    SerialNumber = "DT-XM-004",
                    Name = "Xiaomi 13 Pro 256GB",
                    ExpiredDate = new DateOnly(2026, 9, 30),
                    Unit = "Cái",
                    UnitPrice = 18000000m,
                    ReceivedDate = new DateOnly(2024, 3, 1),
                    PurchasedPrice = 15000000m,
                    ReorderPoint = 20,
                    Image = "https://res.cloudinary.com/demo/xiaomi13.jpg",
                    Description = "Điện thoại Xiaomi cao cấp, camera Leica, sạc nhanh 120W"
                },

                // Laptop & Máy tính
                new Product
                {
                    ProductId = 5,
                    SerialNumber = "LT-DELL-005",
                    Name = "Laptop Dell XPS 13 Plus i7 16GB 512GB",
                    ExpiredDate = new DateOnly(2027, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 42000000m,
                    ReceivedDate = new DateOnly(2024, 1, 20),
                    PurchasedPrice = 38000000m,
                    ReorderPoint = 5,
                    Image = "https://res.cloudinary.com/demo/dell-xps.jpg",
                    Description = "Laptop cao cấp Dell, Intel Core i7 gen 12, màn hình OLED"
                },
                new Product
                {
                    ProductId = 6,
                    SerialNumber = "LT-MAC-006",
                    Name = "MacBook Air M2 13 inch 16GB 512GB",
                    ExpiredDate = new DateOnly(2028, 3, 31),
                    Unit = "Cái",
                    UnitPrice = 38000000m,
                    ReceivedDate = new DateOnly(2024, 2, 15),
                    PurchasedPrice = 34000000m,
                    ReorderPoint = 8,
                    Image = "https://res.cloudinary.com/demo/macbook-air.jpg",
                    Description = "Laptop Apple siêu mỏng, chip M2, pin 18 giờ"
                },
                new Product
                {
                    ProductId = 7,
                    SerialNumber = "LT-ASUS-007",
                    Name = "Laptop Asus ROG Strix G15 RTX 4060",
                    ExpiredDate = new DateOnly(2027, 8, 31),
                    Unit = "Cái",
                    UnitPrice = 35000000m,
                    ReceivedDate = new DateOnly(2024, 3, 10),
                    PurchasedPrice = 30000000m,
                    ReorderPoint = 6,
                    Image = "https://res.cloudinary.com/demo/asus-rog.jpg",
                    Description = "Laptop gaming Asus, RTX 4060, màn hình 165Hz"
                },

                // Phụ kiện điện tử
                new Product
                {
                    ProductId = 8,
                    SerialNumber = "PK-AIRPOD-008",
                    Name = "Tai nghe Apple AirPods Pro 2",
                    ExpiredDate = new DateOnly(2027, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 6500000m,
                    ReceivedDate = new DateOnly(2024, 1, 25),
                    PurchasedPrice = 5500000m,
                    ReorderPoint = 30,
                    Image = "https://res.cloudinary.com/demo/airpods-pro.jpg",
                    Description = "Tai nghe không dây chống ồn chủ động, chip H2"
                },
                new Product
                {
                    ProductId = 9,
                    SerialNumber = "PK-CHUOT-009",
                    Name = "Chuột không dây Logitech MX Master 3S",
                    ExpiredDate = new DateOnly(2028, 6, 30),
                    Unit = "Cái",
                    UnitPrice = 2800000m,
                    ReceivedDate = new DateOnly(2024, 2, 5),
                    PurchasedPrice = 2300000m,
                    ReorderPoint = 50,
                    Image = "https://res.cloudinary.com/demo/mx-master.jpg",
                    Description = "Chuột không dây cao cấp, cảm biến 8000 DPI"
                },
                new Product
                {
                    ProductId = 10,
                    SerialNumber = "PK-BANPHIM-010",
                    Name = "Bàn phím cơ Keychron K8 Pro RGB",
                    ExpiredDate = new DateOnly(2028, 9, 30),
                    Unit = "Cái",
                    UnitPrice = 3500000m,
                    ReceivedDate = new DateOnly(2024, 3, 1),
                    PurchasedPrice = 2800000m,
                    ReorderPoint = 40,
                    Image = "https://res.cloudinary.com/demo/keychron-k8.jpg",
                    Description = "Bàn phím cơ không dây, switch Gateron, đèn RGB"
                },

                // Màn hình & TV
                new Product
                {
                    ProductId = 11,
                    SerialNumber = "MH-LG-011",
                    Name = "Màn hình LG UltraWide 34 inch 144Hz",
                    ExpiredDate = new DateOnly(2028, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 18000000m,
                    ReceivedDate = new DateOnly(2024, 1, 30),
                    PurchasedPrice = 15000000m,
                    ReorderPoint = 5,
                    Image = "https://res.cloudinary.com/demo/lg-ultrawide.jpg",
                    Description = "Màn hình cong 34 inch, độ phân giải QHD, 144Hz"
                },
                new Product
                {
                    ProductId = 12,
                    SerialNumber = "TV-SS-012",
                    Name = "Smart TV Samsung 65 inch QLED 4K",
                    ExpiredDate = new DateOnly(2029, 3, 31),
                    Unit = "Cái",
                    UnitPrice = 28000000m,
                    ReceivedDate = new DateOnly(2024, 2, 20),
                    PurchasedPrice = 24000000m,
                    ReorderPoint = 3,
                    Image = "https://res.cloudinary.com/demo/samsung-qled.jpg",
                    Description = "Smart TV 65 inch, công nghệ QLED, hệ điều hành Tizen"
                },

                // Thiết bị âm thanh
                new Product
                {
                    ProductId = 13,
                    SerialNumber = "AT-SONY-013",
                    Name = "Tai nghe Sony WH-1000XM5",
                    ExpiredDate = new DateOnly(2027, 11, 30),
                    Unit = "Cái",
                    UnitPrice = 9000000m,
                    ReceivedDate = new DateOnly(2024, 3, 5),
                    PurchasedPrice = 7500000m,
                    ReorderPoint = 20,
                    Image = "https://res.cloudinary.com/demo/sony-1000xm5.jpg",
                    Description = "Tai nghe chống ồn cao cấp, pin 30 giờ"
                },
                new Product
                {
                    ProductId = 14,
                    SerialNumber = "LOA-JBL-014",
                    Name = "Loa Bluetooth JBL Charge 5",
                    ExpiredDate = new DateOnly(2028, 8, 31),
                    Unit = "Cái",
                    UnitPrice = 4500000m,
                    ReceivedDate = new DateOnly(2024, 3, 15),
                    PurchasedPrice = 3500000m,
                    ReorderPoint = 25,
                    Image = "https://res.cloudinary.com/demo/jbl-charge5.jpg",
                    Description = "Loa Bluetooth chống nước IP67, pin 20 giờ"
                },

                // Camera & Thiết bị ghi hình
                new Product
                {
                    ProductId = 15,
                    SerialNumber = "CAM-LG-015",
                    Name = "Webcam Logitech Brio 4K Ultra HD",
                    ExpiredDate = new DateOnly(2027, 10, 31),
                    Unit = "Cái",
                    UnitPrice = 5500000m,
                    ReceivedDate = new DateOnly(2024, 4, 1),
                    PurchasedPrice = 4500000m,
                    ReorderPoint = 15,
                    Image = "https://res.cloudinary.com/demo/logitech-brio.jpg",
                    Description = "Webcam 4K, HDR, autofocus, micro tích hợp"
                },
                new Product
                {
                    ProductId = 16,
                    SerialNumber = "CAM-GOPRO-016",
                    Name = "Action Camera GoPro Hero 11 Black",
                    ExpiredDate = new DateOnly(2027, 7, 31),
                    Unit = "Cái",
                    UnitPrice = 12000000m,
                    ReceivedDate = new DateOnly(2024, 4, 10),
                    PurchasedPrice = 10000000m,
                    ReorderPoint = 10,
                    Image = "https://res.cloudinary.com/demo/gopro-hero11.jpg",
                    Description = "Camera hành trình 5.3K, chống nước 10m"
                },

                // Thiết bị mạng
                new Product
                {
                    ProductId = 17,
                    SerialNumber = "MANG-TP-017",
                    Name = "Bộ phát WiFi TP-Link Archer AX73",
                    ExpiredDate = new DateOnly(2028, 5, 31),
                    Unit = "Cái",
                    UnitPrice = 3500000m,
                    ReceivedDate = new DateOnly(2024, 4, 15),
                    PurchasedPrice = 2800000m,
                    ReorderPoint = 20,
                    Image = "https://res.cloudinary.com/demo/tplink-ax73.jpg",
                    Description = "Router WiFi 6 AX5400, 6 ăng-ten"
                },
                new Product
                {
                    ProductId = 18,
                    SerialNumber = "MANG-MESH-018",
                    Name = "Hệ thống Mesh WiFi ASUS ZenWiFi AX6600",
                    ExpiredDate = new DateOnly(2028, 11, 30),
                    Unit = "Bộ",
                    UnitPrice = 12000000m,
                    ReceivedDate = new DateOnly(2024, 5, 1),
                    PurchasedPrice = 10000000m,
                    ReorderPoint = 8,
                    Image = "https://res.cloudinary.com/demo/asus-zenwifi.jpg",
                    Description = "Hệ thống WiFi Mesh, phủ sóng 500m2, WiFi 6"
                },

                // Thiết bị văn phòng
                new Product
                {
                    ProductId = 19,
                    SerialNumber = "VP-MAYIN-019",
                    Name = "Máy in HP LaserJet Pro M404dn",
                    ExpiredDate = new DateOnly(2028, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 9500000m,
                    ReceivedDate = new DateOnly(2024, 5, 10),
                    PurchasedPrice = 8000000m,
                    ReorderPoint = 6,
                    Image = "https://res.cloudinary.com/demo/hp-m404.jpg",
                    Description = "Máy in laser đen trắng, in 2 mặt tự động"
                },
                new Product
                {
                    ProductId = 20,
                    SerialNumber = "VP-SCAN-020",
                    Name = "Máy scan Epson DS-970",
                    ExpiredDate = new DateOnly(2028, 8, 31),
                    Unit = "Cái",
                    UnitPrice = 18000000m,
                    ReceivedDate = new DateOnly(2024, 5, 20),
                    PurchasedPrice = 15000000m,
                    ReorderPoint = 4,
                    Image = "https://res.cloudinary.com/demo/epson-ds970.jpg",
                    Description = "Máy scan tốc độ cao, scan 2 mặt, 85 trang/phút"
                },

                // Phụ kiện & cáp
                new Product
                {
                    ProductId = 21,
                    SerialNumber = "CAP-USBC-021",
                    Name = "Cáp sạc USB-C to Lightning 2m Apple",
                    ExpiredDate = new DateOnly(2029, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 650000m,
                    ReceivedDate = new DateOnly(2024, 6, 1),
                    PurchasedPrice = 450000m,
                    ReorderPoint = 100,
                    Image = "https://res.cloudinary.com/demo/apple-cable.jpg",
                    Description = "Cáp sạc nhanh chính hãng Apple, dài 2m"
                },
                new Product
                {
                    ProductId = 22,
                    SerialNumber = "CAP-HDMI-022",
                    Name = "Cáp HDMI 2.1 8K Belkin 3m",
                    ExpiredDate = new DateOnly(2030, 6, 30),
                    Unit = "Cái",
                    UnitPrice = 850000m,
                    ReceivedDate = new DateOnly(2024, 6, 10),
                    PurchasedPrice = 600000m,
                    ReorderPoint = 80,
                    Image = "https://res.cloudinary.com/demo/belkin-hdmi.jpg",
                    Description = "Cáp HDMI 2.1 hỗ trợ 8K@60Hz, 4K@120Hz"
                },
                new Product
                {
                    ProductId = 23,
                    SerialNumber = "SAC-ANKER-023",
                    Name = "Sạc nhanh Anker PowerPort III 65W",
                    ExpiredDate = new DateOnly(2028, 9, 30),
                    Unit = "Cái",
                    UnitPrice = 1200000m,
                    ReceivedDate = new DateOnly(2024, 6, 15),
                    PurchasedPrice = 900000m,
                    ReorderPoint = 60,
                    Image = "https://res.cloudinary.com/demo/anker-65w.jpg",
                    Description = "Sạc nhanh USB-C 65W, công nghệ GaN"
                },

                // Ổ cứng & USB
                new Product
                {
                    ProductId = 24,
                    SerialNumber = "OCUNG-SSD-024",
                    Name = "Ổ cứng SSD di động Samsung T7 1TB",
                    ExpiredDate = new DateOnly(2029, 3, 31),
                    Unit = "Cái",
                    UnitPrice = 3500000m,
                    ReceivedDate = new DateOnly(2024, 7, 1),
                    PurchasedPrice = 2800000m,
                    ReorderPoint = 30,
                    Image = "https://res.cloudinary.com/demo/samsung-t7.jpg",
                    Description = "SSD di động 1TB, tốc độ 1050MB/s"
                },
                new Product
                {
                    ProductId = 25,
                    SerialNumber = "USB-SAND-025",
                    Name = "USB SanDisk Ultra 3.0 128GB",
                    ExpiredDate = new DateOnly(2030, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 380000m,
                    ReceivedDate = new DateOnly(2024, 7, 10),
                    PurchasedPrice = 250000m,
                    ReorderPoint = 150,
                    Image = "https://res.cloudinary.com/demo/sandisk-usb.jpg",
                    Description = "USB 3.0 128GB, tốc độ đọc 130MB/s"
                },

                // Đồng hồ thông minh & phụ kiện
                new Product
                {
                    ProductId = 26,
                    SerialNumber = "DH-APPLE-026",
                    Name = "Apple Watch Series 8 GPS 45mm",
                    ExpiredDate = new DateOnly(2027, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 11000000m,
                    ReceivedDate = new DateOnly(2024, 7, 15),
                    PurchasedPrice = 9500000m,
                    ReorderPoint = 12,
                    Image = "https://res.cloudinary.com/demo/apple-watch8.jpg",
                    Description = "Đồng hồ thông minh Apple, cảm biến nhiệt độ, ECG"
                },
                new Product
                {
                    ProductId = 27,
                    SerialNumber = "DH-SAMSUNG-027",
                    Name = "Samsung Galaxy Watch 5 Pro",
                    ExpiredDate = new DateOnly(2027, 11, 30),
                    Unit = "Cái",
                    UnitPrice = 9000000m,
                    ReceivedDate = new DateOnly(2024, 7, 20),
                    PurchasedPrice = 7500000m,
                    ReorderPoint = 15,
                    Image = "https://res.cloudinary.com/demo/galaxy-watch5.jpg",
                    Description = "Đồng hồ thông minh Samsung, pin 80 giờ"
                },

                // Thiết bị gia dụng thông minh
                new Product
                {
                    ProductId = 28,
                    SerialNumber = "TBM-GOOGLE-028",
                    Name = "Google Nest Hub Max 10 inch",
                    ExpiredDate = new DateOnly(2028, 10, 31),
                    Unit = "Cái",
                    UnitPrice = 6500000m,
                    ReceivedDate = new DateOnly(2024, 8, 1),
                    PurchasedPrice = 5500000m,
                    ReorderPoint = 10,
                    Image = "https://res.cloudinary.com/demo/nest-hub.jpg",
                    Description = "Màn hình thông minh Google, camera tích hợp"
                },
                new Product
                {
                    ProductId = 29,
                    SerialNumber = "TBM-ALEXA-029",
                    Name = "Amazon Echo Dot thế hệ 5",
                    ExpiredDate = new DateOnly(2028, 7, 31),
                    Unit = "Cái",
                    UnitPrice = 1800000m,
                    ReceivedDate = new DateOnly(2024, 8, 10),
                    PurchasedPrice = 1400000m,
                    ReorderPoint = 40,
                    Image = "https://res.cloudinary.com/demo/echo-dot.jpg",
                    Description = "Loa thông minh Amazon Alexa, điều khiển giọng nói"
                },
                new Product
                {
                    ProductId = 30,
                    SerialNumber = "TBM-ROBO-030",
                    Name = "Robot hút bụi Xiaomi S10+",
                    ExpiredDate = new DateOnly(2028, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 15000000m,
                    ReceivedDate = new DateOnly(2024, 8, 15),
                    PurchasedPrice = 12000000m,
                    ReorderPoint = 5,
                    Image = "https://res.cloudinary.com/demo/xiaomi-s10.jpg",
                    Description = "Robot hút bụi lau nhà, tự làm sạch, ánh xạ laser"
                }
            );

            // Seed Inventory (Liên kết sản phẩm với vị trí kho)
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, ProductId = 1, LocationId = 1, QuantityAvailable = 50, AllocatedQuantity = 10 },
                new Inventory { InventoryId = 2, ProductId = 2, LocationId = 2, QuantityAvailable = 45, AllocatedQuantity = 8 },
                new Inventory { InventoryId = 3, ProductId = 3, LocationId = 3, QuantityAvailable = 30, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 4, ProductId = 4, LocationId = 4, QuantityAvailable = 60, AllocatedQuantity = 15 },
                new Inventory { InventoryId = 5, ProductId = 5, LocationId = 5, QuantityAvailable = 25, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 6, ProductId = 6, LocationId = 6, QuantityAvailable = 35, AllocatedQuantity = 7 },
                new Inventory { InventoryId = 7, ProductId = 7, LocationId = 7, QuantityAvailable = 20, AllocatedQuantity = 4 },
                new Inventory { InventoryId = 8, ProductId = 8, LocationId = 8, QuantityAvailable = 80, AllocatedQuantity = 15 },
                new Inventory { InventoryId = 9, ProductId = 9, LocationId = 9, QuantityAvailable = 120, AllocatedQuantity = 20 },
                new Inventory { InventoryId = 10, ProductId = 10, LocationId = 10, QuantityAvailable = 90, AllocatedQuantity = 18 },
                new Inventory { InventoryId = 11, ProductId = 11, LocationId = 11, QuantityAvailable = 18, AllocatedQuantity = 3 },
                new Inventory { InventoryId = 12, ProductId = 12, LocationId = 12, QuantityAvailable = 12, AllocatedQuantity = 2 },
                new Inventory { InventoryId = 13, ProductId = 13, LocationId = 13, QuantityAvailable = 55, AllocatedQuantity = 10 },
                new Inventory { InventoryId = 14, ProductId = 14, LocationId = 14, QuantityAvailable = 70, AllocatedQuantity = 12 },
                new Inventory { InventoryId = 15, ProductId = 15, LocationId = 15, QuantityAvailable = 40, AllocatedQuantity = 8 },
                new Inventory { InventoryId = 16, ProductId = 16, LocationId = 16, QuantityAvailable = 35, AllocatedQuantity = 6 },
                new Inventory { InventoryId = 17, ProductId = 17, LocationId = 17, QuantityAvailable = 65, AllocatedQuantity = 12 },
                new Inventory { InventoryId = 18, ProductId = 18, LocationId = 18, QuantityAvailable = 28, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 19, ProductId = 19, LocationId = 19, QuantityAvailable = 22, AllocatedQuantity = 4 },
                new Inventory { InventoryId = 20, ProductId = 20, LocationId = 20, QuantityAvailable = 15, AllocatedQuantity = 3 },
                new Inventory { InventoryId = 21, ProductId = 21, LocationId = 21, QuantityAvailable = 300, AllocatedQuantity = 50 },
                new Inventory { InventoryId = 22, ProductId = 22, LocationId = 22, QuantityAvailable = 250, AllocatedQuantity = 40 },
                new Inventory { InventoryId = 23, ProductId = 23, LocationId = 23, QuantityAvailable = 180, AllocatedQuantity = 30 },
                new Inventory { InventoryId = 24, ProductId = 24, LocationId = 24, QuantityAvailable = 95, AllocatedQuantity = 18 },
                new Inventory { InventoryId = 25, ProductId = 25, LocationId = 25, QuantityAvailable = 500, AllocatedQuantity = 80 },
                new Inventory { InventoryId = 26, ProductId = 26, LocationId = 26, QuantityAvailable = 42, AllocatedQuantity = 8 },
                new Inventory { InventoryId = 27, ProductId = 27, LocationId = 27, QuantityAvailable = 50, AllocatedQuantity = 10 },
                new Inventory { InventoryId = 28, ProductId = 28, LocationId = 28, QuantityAvailable = 38, AllocatedQuantity = 7 },
                new Inventory { InventoryId = 29, ProductId = 29, LocationId = 29, QuantityAvailable = 110, AllocatedQuantity = 20 },
                new Inventory { InventoryId = 30, ProductId = 30, LocationId = 30, QuantityAvailable = 25, AllocatedQuantity = 5 }
            );

            // Seed Return Reasons (Lý do trả hàng)
            modelBuilder.Entity<ReturnReason>().HasData(
                new ReturnReason
                {
                    ReasonId = 1,
                    ReasonCode = "HONG_HAO",
                    Description = "Sản phẩm bị hỏng hóc trong quá trình vận chuyển"
                },
                new ReturnReason
                {
                    ReasonId = 2,
                    ReasonCode = "LOI_KY_THUAT",
                    Description = "Sản phẩm bị lỗi kỹ thuật, không hoạt động đúng"
                },
                new ReturnReason
                {
                    ReasonId = 3,
                    ReasonCode = "SAI_HANG",
                    Description = "Giao sai sản phẩm, không đúng đơn hàng"
                },
                new ReturnReason
                {
                    ReasonId = 4,
                    ReasonCode = "HET_HAN",
                    Description = "Sản phẩm hết hạn hoặc gần hết hạn sử dụng"
                },
                new ReturnReason
                {
                    ReasonId = 5,
                    ReasonCode = "CHAT_LUONG",
                    Description = "Chất lượng sản phẩm không đạt tiêu chuẩn"
                },
                new ReturnReason
                {
                    ReasonId = 6,
                    ReasonCode = "KHACH_TRA",
                    Description = "Khách hàng yêu cầu trả hàng hoặc đổi sản phẩm"
                },
                new ReturnReason
                {
                    ReasonId = 7,
                    ReasonCode = "TON_KHO",
                    Description = "Trả hàng do tồn kho quá nhiều"
                },
                new ReturnReason
                {
                    ReasonId = 8,
                    ReasonCode = "KHONG_DUNG_MO_TA",
                    Description = "Sản phẩm không đúng với mô tả hoặc thông số kỹ thuật"
                },
                new ReturnReason
                {
                    ReasonId = 9,
                    ReasonCode = "BAO_BI_HU",
                    Description = "Bao bì sản phẩm bị hư hỏng, rách, không nguyên vẹn"
                },
                new ReturnReason
                {
                    ReasonId = 10,
                    ReasonCode = "THIEU_PHU_KIEN",
                    Description = "Sản phẩm thiếu phụ kiện đi kèm"
                }
            );
        }
    }
}

