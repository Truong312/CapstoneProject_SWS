using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SWS.BusinessObjects.Models;

namespace SWS.BusinessObjects.Extensions
{
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Seed initial data vào database khi app khởi động
        /// </summary>
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartWarehouseDbContext>();

            // Đảm bảo database đã được tạo và migrations đã apply
            await context.Database.MigrateAsync();

            // Kiểm tra nếu đã có data thì không seed nữa
            if (await context.Users.AnyAsync())
            {
                Console.WriteLine("[Seeding] Database đã có dữ liệu. Bỏ qua seeding.");
                return;
            }

            Console.WriteLine("[Seeding] Bắt đầu seed dữ liệu...");

            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    FullName = "Nguyễn Văn Admin",
                    Email = "admin@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Phone = "0901234567",
                    Address = "123 Đường Láng, Đống Đa, Hà Nội",
                    Role = 1
                },
                new User
                {
                    FullName = "Trần Thị Lan",
                    Email = "quanly@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("QuanLy@123"),
                    Phone = "0902345678",
                    Address = "456 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                    Role = 2
                },
                new User
                {
                    FullName = "Lê Văn Hùng",
                    Email = "nhanvien1@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0903456789",
                    Address = "789 Giải Phóng, Hai Bà Trưng, Hà Nội",
                    Role = 3
                },
                new User
                {
                    FullName = "Phạm Thị Mai",
                    Email = "nhanvien2@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0904567890",
                    Address = "321 Cầu Giấy, Hà Nội",
                    Role = 3
                },
                new User
                {
                    FullName = "Hoàng Minh Tuấn",
                    Email = "nhanvien3@khovietnam.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("NhanVien@123"),
                    Phone = "0905678901",
                    Address = "100 Tây Sơn, Đống Đa, Hà Nội",
                    Role = 3
                }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {users.Count} users");

            // Seed Business Partners
            var partners = new List<BusinessPartner>
            {
                new BusinessPartner
                {
                    Name = "Công ty TNHH Điện Tử Việt Nam",
                    Email = "contact@dientuVN.com",
                    Phone = "0241234567",
                    Address = "100 Khu Công Nghệ Cao, Quận 9, TP.HCM",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    Name = "Tập Đoàn Công Nghệ FPT",
                    Email = "sales@fpt.com.vn",
                    Phone = "0242345678",
                    Address = "200 Lê Văn Lương, Thanh Xuân, Hà Nội",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    Name = "Nhà Phân Phối Thế Giới Di Động",
                    Email = "orders@thegioididong.com",
                    Phone = "0243456789",
                    Address = "300 Trần Hưng Đạo, Quận 1, TP.HCM",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    Name = "Siêu Thị Điện Máy Xanh",
                    Email = "procurement@dienmayxanh.com",
                    Phone = "0244567890",
                    Address = "400 Nguyễn Văn Linh, Quận 7, TP.HCM",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    Name = "Công ty CP Phân Phối Hàng Hóa Việt",
                    Email = "info@phanphoiviet.com",
                    Phone = "0245678901",
                    Address = "500 Hải Phòng, Đà Nẵng",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    Name = "Nhà Cung Cấp Linh Kiện Điện Tử ABC",
                    Email = "sales@linhkienabc.com",
                    Phone = "0246789012",
                    Address = "600 Khu Công Nghiệp Tân Thuận, TP.HCM",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    Name = "Chuỗi Siêu Thị VinMart",
                    Email = "order@vinmart.com.vn",
                    Phone = "0247890123",
                    Address = "700 Phạm Văn Đồng, Bắc Từ Liêm, Hà Nội",
                    Type = "Customer"
                }
            };
            await context.BusinessPartners.AddRangeAsync(partners);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {partners.Count} business partners");

            // Seed Locations
            var locations = new List<Location>();
            string[] shelves = { "A", "B", "C", "D", "E" };
            foreach (var shelf in shelves)
            {
                for (int col = 1; col <= 6; col++)
                {
                    for (int row = 1; row <= 6; row++)
                    {
                        locations.Add(new Location
                        {
                            ShelfId = shelf,
                            ColumnNumber = col,
                            RowNumber = row,
                            Type = row <= 2 ? "Tầng Thấp" : (row <= 4 ? "Tầng Trung" : "Tầng Cao"),
                            IsFull = false
                        });
                    }
                }
            }
            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {locations.Count} vị trí kho");

            // Seed Products (30 sản phẩm đa dạng)
            var products = new List<Product>
            {
                // Điện thoại & Tablet
                new Product
                {
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
                new Product
                {
                    SerialNumber = "DT-OPPO-005",
                    Name = "OPPO Find X6 Pro 5G",
                    ExpiredDate = new DateOnly(2027, 3, 31),
                    Unit = "Cái",
                    UnitPrice = 22000000m,
                    ReceivedDate = new DateOnly(2024, 3, 15),
                    PurchasedPrice = 19000000m,
                    ReorderPoint = 12,
                    Image = "https://res.cloudinary.com/demo/oppo-findx6.jpg",
                    Description = "Điện thoại OPPO flagship, camera Hasselblad, màn hình AMOLED 120Hz"
                },

                // Laptop & Máy tính
                new Product
                {
                    SerialNumber = "LT-DELL-006",
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
                    SerialNumber = "LT-MAC-007",
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
                    SerialNumber = "LT-ASUS-008",
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
                new Product
                {
                    SerialNumber = "LT-HP-009",
                    Name = "Laptop HP Pavilion 15 i5 8GB 256GB",
                    ExpiredDate = new DateOnly(2027, 6, 30),
                    Unit = "Cái",
                    UnitPrice = 18000000m,
                    ReceivedDate = new DateOnly(2024, 4, 1),
                    PurchasedPrice = 15000000m,
                    ReorderPoint = 10,
                    Image = "https://res.cloudinary.com/demo/hp-pavilion.jpg",
                    Description = "Laptop văn phòng HP, Intel Core i5 gen 11, SSD 256GB"
                },
                new Product
                {
                    SerialNumber = "LT-LENOVO-010",
                    Name = "Laptop Lenovo ThinkPad X1 Carbon Gen 11",
                    ExpiredDate = new DateOnly(2028, 1, 31),
                    Unit = "Cái",
                    UnitPrice = 45000000m,
                    ReceivedDate = new DateOnly(2024, 4, 15),
                    PurchasedPrice = 40000000m,
                    ReorderPoint = 4,
                    Image = "https://res.cloudinary.com/demo/thinkpad-x1.jpg",
                    Description = "Laptop doanh nhân Lenovo, Intel Core i7 gen 13, siêu nhẹ 1.12kg"
                },

                // Phụ kiện điện tử
                new Product
                {
                    SerialNumber = "PK-AIRPOD-011",
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
                    SerialNumber = "PK-CHUOT-012",
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
                    SerialNumber = "PK-BANPHIM-013",
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
                    SerialNumber = "MH-LG-014",
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
                    SerialNumber = "TV-SS-015",
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
                new Product
                {
                    SerialNumber = "MH-DELL-016",
                    Name = "Màn hình Dell UltraSharp 27 inch 4K",
                    ExpiredDate = new DateOnly(2028, 8, 31),
                    Unit = "Cái",
                    UnitPrice = 12000000m,
                    ReceivedDate = new DateOnly(2024, 3, 5),
                    PurchasedPrice = 10000000m,
                    ReorderPoint = 8,
                    Image = "https://res.cloudinary.com/demo/dell-ultrasharp.jpg",
                    Description = "Màn hình chuyên nghiệp Dell, độ phân giải 4K, 99% sRGB"
                },

                // Thiết bị âm thanh
                new Product
                {
                    SerialNumber = "AT-SONY-017",
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
                    SerialNumber = "LOA-JBL-018",
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
                    SerialNumber = "CAM-LG-019",
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
                    SerialNumber = "CAM-GOPRO-020",
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
                    SerialNumber = "MANG-TP-021",
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
                    SerialNumber = "MANG-MESH-022",
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
                    SerialNumber = "VP-MAYIN-023",
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
                    SerialNumber = "VP-SCAN-024",
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
                    SerialNumber = "CAP-USBC-025",
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
                    SerialNumber = "CAP-HDMI-026",
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
                    SerialNumber = "SAC-ANKER-027",
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
                    SerialNumber = "OCUNG-SSD-028",
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
                    SerialNumber = "USB-SAND-029",
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

                // Đồng hồ thông minh
                new Product
                {
                    SerialNumber = "DH-APPLE-030",
                    Name = "Apple Watch Series 8 GPS 45mm",
                    ExpiredDate = new DateOnly(2027, 12, 31),
                    Unit = "Cái",
                    UnitPrice = 11000000m,
                    ReceivedDate = new DateOnly(2024, 7, 15),
                    PurchasedPrice = 9500000m,
                    ReorderPoint = 12,
                    Image = "https://res.cloudinary.com/demo/apple-watch8.jpg",
                    Description = "Đồng hồ thông minh Apple, cảm biến nhiệt độ, ECG"
                }
            };
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {products.Count} sản phẩm");

            // Seed Inventory
            var inventories = new List<Inventory>();
            for (int i = 0; i < products.Count && i < 30; i++)
            {
                inventories.Add(new Inventory
                {
                    ProductId = products[i].ProductId,
                    LocationId = locations[i].LocationId,
                    QuantityAvailable = 50 + (i * 10),
                    AllocatedQuantity = 5 + (i * 2)
                });
            }
            await context.Inventories.AddRangeAsync(inventories);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {inventories.Count} inventory records");

            // Seed Return Reasons
            var returnReasons = new List<ReturnReason>
            {
                new ReturnReason
                {
                    ReasonCode = "HONG_HAO",
                    Description = "Sản phẩm bị hỏng hóc trong quá trình vận chuyển"
                },
                new ReturnReason
                {
                    ReasonCode = "LOI_KY_THUAT",
                    Description = "Sản phẩm bị lỗi kỹ thuật, không hoạt động đúng"
                },
                new ReturnReason
                {
                    ReasonCode = "SAI_HANG",
                    Description = "Giao sai sản phẩm, không đúng đơn hàng"
                },
                new ReturnReason
                {
                    ReasonCode = "HET_HAN",
                    Description = "Sản phẩm hết hạn hoặc gần hết hạn sử dụng"
                },
                new ReturnReason
                {
                    ReasonCode = "CHAT_LUONG",
                    Description = "Chất lượng sản phẩm không đạt tiêu chuẩn"
                },
                new ReturnReason
                {
                    ReasonCode = "KHACH_TRA",
                    Description = "Khách hàng yêu cầu trả hàng hoặc đổi sản phẩm"
                },
                new ReturnReason
                {
                    ReasonCode = "TON_KHO",
                    Description = "Trả hàng do tồn kho quá nhiều"
                },
                new ReturnReason
                {
                    ReasonCode = "KHONG_DUNG_MO_TA",
                    Description = "Sản phẩm không đúng với mô tả hoặc thông số kỹ thuật"
                },
                new ReturnReason
                {
                    ReasonCode = "BAO_BI_HU",
                    Description = "Bao bì sản phẩm bị hư hỏng, rách, không nguyên vẹn"
                },
                new ReturnReason
                {
                    ReasonCode = "THIEU_PHU_KIEN",
                    Description = "Sản phẩm thiếu phụ kiện đi kèm"
                }
            };
            await context.ReturnReasons.AddRangeAsync(returnReasons);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seeding] Đã thêm {returnReasons.Count} lý do trả hàng");

            Console.WriteLine("[Seeding] Hoàn thành seeding dữ liệu!");
        }
    }
}

