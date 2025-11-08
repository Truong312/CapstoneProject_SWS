using Microsoft.EntityFrameworkCore;
using SWS.BusinessObjects.Models;
using System;

namespace SWS.ApiCore.Extensions
{
    public static class DataSeedingConfig
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Seed Users (Admin, Manager, Staff)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Admin User",
                    Email = "admin@warehouse.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Phone = "0901234567",
                    Address = "123 Admin Street, Hanoi",
                    Role = 1 // Admin
                },
                new User
                {
                    UserId = 2,
                    FullName = "Warehouse Manager",
                    Email = "manager@warehouse.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    Phone = "0902345678",
                    Address = "456 Manager Street, Hanoi",
                    Role = 2 // Manager
                },
                new User
                {
                    UserId = 3,
                    FullName = "Staff Member 1",
                    Email = "staff1@warehouse.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                    Phone = "0903456789",
                    Address = "789 Staff Street, Hanoi",
                    Role = 3 // Staff
                },
                new User
                {
                    UserId = 4,
                    FullName = "Staff Member 2",
                    Email = "staff2@warehouse.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                    Phone = "0904567890",
                    Address = "321 Staff Avenue, Hanoi",
                    Role = 3 // Staff
                }
            );

            // Seed Business Partners (Suppliers & Customers)
            modelBuilder.Entity<BusinessPartner>().HasData(
                new BusinessPartner
                {
                    PartnerId = 1,
                    Name = "Tech Supplies Co., Ltd",
                    Email = "contact@techsupplies.com",
                    Phone = "0241234567",
                    Address = "100 Technology Park, Hanoi",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    PartnerId = 2,
                    Name = "Global Electronics Supplier",
                    Email = "sales@globalelectronics.com",
                    Phone = "0242345678",
                    Address = "200 Industrial Zone, HCMC",
                    Type = "Supplier"
                },
                new BusinessPartner
                {
                    PartnerId = 3,
                    Name = "Retail Chain A",
                    Email = "orders@retailchaina.com",
                    Phone = "0243456789",
                    Address = "300 Retail Street, Hanoi",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    PartnerId = 4,
                    Name = "E-commerce Platform B",
                    Email = "procurement@ecommerceb.com",
                    Phone = "0244567890",
                    Address = "400 Digital Avenue, HCMC",
                    Type = "Customer"
                },
                new BusinessPartner
                {
                    PartnerId = 5,
                    Name = "Wholesale Distributor C",
                    Email = "info@wholesalec.com",
                    Phone = "0245678901",
                    Address = "500 Distribution Center, Danang",
                    Type = "Customer"
                }
            );

            // Seed Locations (Warehouse shelves and positions)
            var locations = new List<Location>();
            int locationId = 1;

            // Create warehouse layout: 5 shelves (A-E), each with 4 columns and 5 rows
            string[] shelves = { "A", "B", "C", "D", "E" };
            foreach (var shelf in shelves)
            {
                for (int col = 1; col <= 4; col++)
                {
                    for (int row = 1; row <= 5; row++)
                    {
                        locations.Add(new Location
                        {
                            LocationId = locationId++,
                            ShelfId = shelf,
                            ColumnNumber = col,
                            RowNumber = row,
                            Type = row <= 2 ? "Ground" : "Elevated",
                            IsFull = false
                        });
                    }
                }
            }

            modelBuilder.Entity<Location>().HasData(locations);

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    SerialNumber = "LAPTOP-001",
                    Name = "Dell Latitude 5420 Laptop",
                    ExpiredDate = new DateOnly(2026, 12, 31),
                    Unit = "Unit",
                    UnitPrice = 25000000m,
                    ReceivedDate = new DateOnly(2024, 1, 15),
                    PurchasedPrice = 20000000m,
                    ReorderPoint = 10,
                    Image = "https://res.cloudinary.com/demo/laptop-dell.jpg",
                    Description = "Business laptop with Intel i7, 16GB RAM, 512GB SSD"
                },
                new Product
                {
                    ProductId = 2,
                    SerialNumber = "MOUSE-001",
                    Name = "Logitech MX Master 3 Mouse",
                    ExpiredDate = new DateOnly(2027, 6, 30),
                    Unit = "Unit",
                    UnitPrice = 2500000m,
                    ReceivedDate = new DateOnly(2024, 2, 1),
                    PurchasedPrice = 2000000m,
                    ReorderPoint = 50,
                    Image = "https://res.cloudinary.com/demo/mouse-logitech.jpg",
                    Description = "Wireless ergonomic mouse"
                },
                new Product
                {
                    ProductId = 3,
                    SerialNumber = "KEYBOARD-001",
                    Name = "Keychron K2 Mechanical Keyboard",
                    ExpiredDate = new DateOnly(2027, 12, 31),
                    Unit = "Unit",
                    UnitPrice = 3000000m,
                    ReceivedDate = new DateOnly(2024, 2, 10),
                    PurchasedPrice = 2500000m,
                    ReorderPoint = 30,
                    Image = "https://res.cloudinary.com/demo/keyboard-keychron.jpg",
                    Description = "Wireless mechanical keyboard with RGB"
                },
                new Product
                {
                    ProductId = 4,
                    SerialNumber = "MONITOR-001",
                    Name = "LG UltraWide 34 inch Monitor",
                    ExpiredDate = new DateOnly(2028, 3, 31),
                    Unit = "Unit",
                    UnitPrice = 15000000m,
                    ReceivedDate = new DateOnly(2024, 3, 1),
                    PurchasedPrice = 12000000m,
                    ReorderPoint = 5,
                    Image = "https://res.cloudinary.com/demo/monitor-lg.jpg",
                    Description = "34-inch ultrawide curved monitor"
                },
                new Product
                {
                    ProductId = 5,
                    SerialNumber = "HEADSET-001",
                    Name = "Sony WH-1000XM5 Headphones",
                    ExpiredDate = new DateOnly(2027, 9, 30),
                    Unit = "Unit",
                    UnitPrice = 8000000m,
                    ReceivedDate = new DateOnly(2024, 3, 15),
                    PurchasedPrice = 6500000m,
                    ReorderPoint = 20,
                    Image = "https://res.cloudinary.com/demo/headset-sony.jpg",
                    Description = "Noise-cancelling wireless headphones"
                },
                new Product
                {
                    ProductId = 6,
                    SerialNumber = "WEBCAM-001",
                    Name = "Logitech Brio 4K Webcam",
                    ExpiredDate = new DateOnly(2027, 8, 31),
                    Unit = "Unit",
                    UnitPrice = 5000000m,
                    ReceivedDate = new DateOnly(2024, 4, 1),
                    PurchasedPrice = 4000000m,
                    ReorderPoint = 15,
                    Image = "https://res.cloudinary.com/demo/webcam-logitech.jpg",
                    Description = "4K webcam with HDR support"
                },
                new Product
                {
                    ProductId = 7,
                    SerialNumber = "TABLET-001",
                    Name = "Apple iPad Pro 12.9 inch",
                    ExpiredDate = new DateOnly(2028, 1, 31),
                    Unit = "Unit",
                    UnitPrice = 30000000m,
                    ReceivedDate = new DateOnly(2024, 4, 15),
                    PurchasedPrice = 25000000m,
                    ReorderPoint = 8,
                    Image = "https://res.cloudinary.com/demo/tablet-apple.jpg",
                    Description = "Professional tablet with M2 chip"
                },
                new Product
                {
                    ProductId = 8,
                    SerialNumber = "ROUTER-001",
                    Name = "TP-Link AX6000 WiFi Router",
                    ExpiredDate = new DateOnly(2028, 6, 30),
                    Unit = "Unit",
                    UnitPrice = 7000000m,
                    ReceivedDate = new DateOnly(2024, 5, 1),
                    PurchasedPrice = 5500000m,
                    ReorderPoint = 12,
                    Image = "https://res.cloudinary.com/demo/router-tplink.jpg",
                    Description = "WiFi 6 dual-band router"
                },
                new Product
                {
                    ProductId = 9,
                    SerialNumber = "PRINTER-001",
                    Name = "HP LaserJet Pro M404dn",
                    ExpiredDate = new DateOnly(2028, 12, 31),
                    Unit = "Unit",
                    UnitPrice = 9000000m,
                    ReceivedDate = new DateOnly(2024, 5, 15),
                    PurchasedPrice = 7500000m,
                    ReorderPoint = 6,
                    Image = "https://res.cloudinary.com/demo/printer-hp.jpg",
                    Description = "Monochrome laser printer with duplex"
                },
                new Product
                {
                    ProductId = 10,
                    SerialNumber = "CABLE-001",
                    Name = "USB-C to HDMI Cable 2m",
                    ExpiredDate = new DateOnly(2029, 12, 31),
                    Unit = "Piece",
                    UnitPrice = 500000m,
                    ReceivedDate = new DateOnly(2024, 6, 1),
                    PurchasedPrice = 300000m,
                    ReorderPoint = 100,
                    Image = "https://res.cloudinary.com/demo/cable-usbc.jpg",
                    Description = "High-speed USB-C to HDMI cable"
                }
            );

            // Seed Inventory (linking Products to Locations)
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, ProductId = 1, LocationId = 1, QuantityAvailable = 25, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 2, ProductId = 2, LocationId = 2, QuantityAvailable = 150, AllocatedQuantity = 10 },
                new Inventory { InventoryId = 3, ProductId = 3, LocationId = 3, QuantityAvailable = 80, AllocatedQuantity = 15 },
                new Inventory { InventoryId = 4, ProductId = 4, LocationId = 4, QuantityAvailable = 12, AllocatedQuantity = 3 },
                new Inventory { InventoryId = 5, ProductId = 5, LocationId = 5, QuantityAvailable = 45, AllocatedQuantity = 8 },
                new Inventory { InventoryId = 6, ProductId = 6, LocationId = 6, QuantityAvailable = 30, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 7, ProductId = 7, LocationId = 7, QuantityAvailable = 18, AllocatedQuantity = 2 },
                new Inventory { InventoryId = 8, ProductId = 8, LocationId = 8, QuantityAvailable = 22, AllocatedQuantity = 4 },
                new Inventory { InventoryId = 9, ProductId = 9, LocationId = 9, QuantityAvailable = 14, AllocatedQuantity = 2 },
                new Inventory { InventoryId = 10, ProductId = 10, LocationId = 10, QuantityAvailable = 500, AllocatedQuantity = 50 }
            );

            // Seed Return Reasons
            modelBuilder.Entity<ReturnReason>().HasData(
                new ReturnReason
                {
                    ReasonId = 1,
                    ReasonCode = "DAMAGED",
                    Description = "Product damaged during shipping or handling"
                },
                new ReturnReason
                {
                    ReasonId = 2,
                    ReasonCode = "DEFECTIVE",
                    Description = "Product is defective or not working properly"
                },
                new ReturnReason
                {
                    ReasonId = 3,
                    ReasonCode = "WRONG_ITEM",
                    Description = "Wrong item was shipped"
                },
                new ReturnReason
                {
                    ReasonId = 4,
                    ReasonCode = "EXPIRED",
                    Description = "Product has expired or nearing expiration"
                },
                new ReturnReason
                {
                    ReasonId = 5,
                    ReasonCode = "QUALITY_ISSUE",
                    Description = "Product quality does not meet standards"
                },
                new ReturnReason
                {
                    ReasonId = 6,
                    ReasonCode = "CUSTOMER_REQUEST",
                    Description = "Customer requested return or exchange"
                },
                new ReturnReason
                {
                    ReasonId = 7,
                    ReasonCode = "OVERSTOCK",
                    Description = "Excess inventory return"
                },
                new ReturnReason
                {
                    ReasonId = 8,
                    ReasonCode = "MISMATCH",
                    Description = "Product does not match description or specifications"
                }
            );

            // Additional mock data for Import/Export/Return flows and near-expired products

            // Seed Import Orders + Details
            modelBuilder.Entity<ImportOrder>().HasData(
                new ImportOrder
                {
                    ImportOrderId = 1,
                    InvoiceNumber = "IMP-20251101-001",
                    OrderDate = new DateOnly(2025, 11, 1),
                    ProviderId = 1,
                    CreatedDate = new DateOnly(2025, 11, 1),
                    Status = "Pending",
                    CreatedBy = 2
                },
                new ImportOrder
                {
                    ImportOrderId = 2,
                    InvoiceNumber = "IMP-20251015-001",
                    OrderDate = new DateOnly(2025, 10, 15),
                    ProviderId = 2,
                    CreatedDate = new DateOnly(2025, 10, 15),
                    Status = "Completed",
                    CreatedBy = 3
                }
            );

            modelBuilder.Entity<ImportDetail>().HasData(
                new ImportDetail { ImportDetailId = 1, ImportOrderId = 1, ProductId = 1, Quantity = 10, ImportPrice = 20000000m },
                new ImportDetail { ImportDetailId = 2, ImportOrderId = 1, ProductId = 2, Quantity = 20, ImportPrice = 2000000m },
                new ImportDetail { ImportDetailId = 3, ImportOrderId = 2, ProductId = 3, Quantity = 5, ImportPrice = 2500000m },
                new ImportDetail { ImportDetailId = 4, ImportOrderId = 2, ProductId = 4, Quantity = 2, ImportPrice = 12000000m }
            );

            // Seed Export Orders + Details
            modelBuilder.Entity<ExportOrder>().HasData(
                new ExportOrder
                {
                    ExportOrderId = 1,
                    InvoiceNumber = "EXP-20251105-001",
                    OrderDate = new DateOnly(2025, 11, 5),
                    CustomerId = 3,
                    Currency = "VND",
                    CreatedDate = new DateOnly(2025, 11, 5),
                    ShippedDate = new DateOnly(2025, 11, 7),
                    ShippedAddress = "123 Customer St",
                    TaxRate = 0.05m,
                    TaxAmount = 12500000m * 0.05m, // example calculation
                    TotalPayment = 12500000m * 1.05m,
                    Description = "Order to Retail Chain A",
                    Status = "Shipped",
                    CreatedBy = 2
                },
                new ExportOrder
                {
                    ExportOrderId = 2,
                    InvoiceNumber = "EXP-20251106-001",
                    OrderDate = new DateOnly(2025, 11, 6),
                    CustomerId = 4,
                    Currency = "USD",
                    CreatedDate = new DateOnly(2025, 11, 6),
                    Status = "Pending",
                    CreatedBy = 3
                }
            );

            modelBuilder.Entity<ExportDetail>().HasData(
                new ExportDetail { ExportDetailId = 1, ExportOrderId = 1, ProductId = 2, Quantity = 5, TotalPrice = 2500000m * 5 },
                new ExportDetail { ExportDetailId = 2, ExportOrderId = 2, ProductId = 1, Quantity = 1, TotalPrice = 25000000m }
            );

            // Seed Return Orders + Return Details (different statuses for search)
            modelBuilder.Entity<ReturnOrder>().HasData(
                new ReturnOrder
                {
                    ReturnOrderId = 1,
                    ExportOrderId = 1,
                    CheckedBy = 3,
                    ReviewedBy = 2,
                    CheckInTime = new DateTime(2025, 11, 7, 10, 0, 0),
                    Status = "Pending",
                    Note = "Customer reported damaged items"
                },
                new ReturnOrder
                {
                    ReturnOrderId = 2,
                    ExportOrderId = 2,
                    CheckedBy = 3,
                    ReviewedBy = 2,
                    CheckInTime = new DateTime(2025, 11, 8, 9, 30, 0),
                    Status = "Processed",
                    Note = "Quality issue"
                }
            );

            modelBuilder.Entity<ReturnOrderDetail>().HasData(
                new ReturnOrderDetail { ReturnDetailId = 1, ReturnOrderId = 1, ProductId = 2, Quantity = 1, Note = "Broken on arrival", ReasonId = 1, ActionId = null, LocationId = 5 },
                new ReturnOrderDetail { ReturnDetailId = 2, ReturnOrderId = 2, ProductId = 4, Quantity = 1, Note = "Wrong color", ReasonId = 3, ActionId = null, LocationId = 6 }
            );

            // Seed additional products that are near-expired for "near expired" notifications
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 11,
                    SerialNumber = "PERISH-001",
                    Name = "Perishable Item A",
                    ExpiredDate = new DateOnly(2025, 11, 12),
                    Unit = "Box",
                    UnitPrice = 100000m,
                    ReceivedDate = new DateOnly(2025, 9, 1),
                    PurchasedPrice = 80000m,
                    ReorderPoint = 20,
                    Image = "",
                    Description = "Perishable product near expiry"
                },
                new Product
                {
                    ProductId = 12,
                    SerialNumber = "PERISH-002",
                    Name = "Perishable Item B",
                    ExpiredDate = new DateOnly(2025, 11, 20),
                    Unit = "Box",
                    UnitPrice = 120000m,
                    ReceivedDate = new DateOnly(2025, 9, 1),
                    PurchasedPrice = 90000m,
                    ReorderPoint = 30,
                    Image = "",
                    Description = "Perishable product near expiry"
                }
            );

            // Inventory entries for the new near-expired products
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 11, ProductId = 11, LocationId = 11, QuantityAvailable = 50, AllocatedQuantity = 0 },
                new Inventory { InventoryId = 12, ProductId = 12, LocationId = 12, QuantityAvailable = 30, AllocatedQuantity = 0 }
            );

            // ----------------------------------------------------------------
            // Additional bulk mock data to make lists/details/search/dashboard useful
            // ----------------------------------------------------------------

            // Add more perishable and regular products (ProductId 13..20)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 13,
                    SerialNumber = "FOOD-001",
                    Name = "Snack Pack A",
                    ExpiredDate = new DateOnly(2025, 11, 10), // very near-expiry
                    Unit = "Pack",
                    UnitPrice = 50000m,
                    ReceivedDate = new DateOnly(2025, 9, 15),
                    PurchasedPrice = 40000m,
                    ReorderPoint = 100,
                    Image = "",
                    Description = "Snack pack near expiry"
                },
                new Product
                {
                    ProductId = 14,
                    SerialNumber = "FOOD-002",
                    Name = "Beverage B",
                    ExpiredDate = new DateOnly(2025, 11, 25),
                    Unit = "Bottle",
                    UnitPrice = 30000m,
                    ReceivedDate = new DateOnly(2025, 9, 10),
                    PurchasedPrice = 20000m,
                    ReorderPoint = 200,
                    Image = "",
                    Description = "Beverage near expiry"
                },
                new Product
                {
                    ProductId = 15,
                    SerialNumber = "ACCESS-001",
                    Name = "Phone Case",
                    ExpiredDate = new DateOnly(2030, 1, 1),
                    Unit = "Unit",
                    UnitPrice = 150000m,
                    ReceivedDate = new DateOnly(2025, 8, 1),
                    PurchasedPrice = 100000m,
                    ReorderPoint = 50,
                    Image = "",
                    Description = "Phone protective case"
                },
                new Product
                {
                    ProductId = 16,
                    SerialNumber = "CABLE-002",
                    Name = "HDMI Cable 3m",
                    ExpiredDate = new DateOnly(2029, 12, 31),
                    Unit = "Piece",
                    UnitPrice = 80000m,
                    ReceivedDate = new DateOnly(2025, 7, 1),
                    PurchasedPrice = 40000m,
                    ReorderPoint = 150,
                    Image = "",
                    Description = "HDMI cable 3 meters"
                },
                new Product
                {
                    ProductId = 17,
                    SerialNumber = "PERF-003",
                    Name = "Perishable C",
                    ExpiredDate = new DateOnly(2025, 11, 18),
                    Unit = "Box",
                    UnitPrice = 90000m,
                    ReceivedDate = new DateOnly(2025, 9, 5),
                    PurchasedPrice = 70000m,
                    ReorderPoint = 40,
                    Image = "",
                    Description = "Perishable near expiry"
                },
                new Product
                {
                    ProductId = 18,
                    SerialNumber = "PERF-004",
                    Name = "Perishable D",
                    ExpiredDate = new DateOnly(2025, 12, 5),
                    Unit = "Box",
                    UnitPrice = 110000m,
                    ReceivedDate = new DateOnly(2025, 8, 20),
                    PurchasedPrice = 90000m,
                    ReorderPoint = 25,
                    Image = "",
                    Description = "Perishable product"
                },
                new Product
                {
                    ProductId = 19,
                    SerialNumber = "GADGET-001",
                    Name = "USB Hub 4-port",
                    ExpiredDate = new DateOnly(2030, 6, 30),
                    Unit = "Unit",
                    UnitPrice = 350000m,
                    ReceivedDate = new DateOnly(2025, 6, 10),
                    PurchasedPrice = 200000m,
                    ReorderPoint = 20,
                    Image = "",
                    Description = "USB hub 4-port"
                },
                new Product
                {
                    ProductId = 20,
                    SerialNumber = "CHARGER-001",
                    Name = "Fast Charger 65W",
                    ExpiredDate = new DateOnly(2031, 1, 1),
                    Unit = "Unit",
                    UnitPrice = 450000m,
                    ReceivedDate = new DateOnly(2025, 5, 10),
                    PurchasedPrice = 300000m,
                    ReorderPoint = 30,
                    Image = "",
                    Description = "65W fast charger"
                }
            );

            // Inventory for the new products (InventoryId 13..20)
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 13, ProductId = 13, LocationId = 13, QuantityAvailable = 120, AllocatedQuantity = 10 },
                new Inventory { InventoryId = 14, ProductId = 14, LocationId = 14, QuantityAvailable = 80, AllocatedQuantity = 5 },
                new Inventory { InventoryId = 15, ProductId = 15, LocationId = 15, QuantityAvailable = 40, AllocatedQuantity = 0 },
                new Inventory { InventoryId = 16, ProductId = 16, LocationId = 16, QuantityAvailable = 200, AllocatedQuantity = 30 },
                new Inventory { InventoryId = 17, ProductId = 17, LocationId = 17, QuantityAvailable = 60, AllocatedQuantity = 8 },
                new Inventory { InventoryId = 18, ProductId = 18, LocationId = 18, QuantityAvailable = 35, AllocatedQuantity = 2 },
                new Inventory { InventoryId = 19, ProductId = 19, LocationId = 19, QuantityAvailable = 25, AllocatedQuantity = 0 },
                new Inventory { InventoryId = 20, ProductId = 20, LocationId = 20, QuantityAvailable = 70, AllocatedQuantity = 5 }
            );

            // Bulk Import Orders (ImportOrderId 3..12) and ImportDetails (ImportDetailId 5..24)
            modelBuilder.Entity<ImportOrder>().HasData(
                new ImportOrder { ImportOrderId = 3, InvoiceNumber = "IMP-20250901-001", OrderDate = new DateOnly(2025, 9, 1), ProviderId = 1, CreatedDate = new DateOnly(2025, 9, 1), Status = "Completed", CreatedBy = 2 },
                new ImportOrder { ImportOrderId = 4, InvoiceNumber = "IMP-20250915-001", OrderDate = new DateOnly(2025, 9, 15), ProviderId = 2, CreatedDate = new DateOnly(2025, 9, 15), Status = "Completed", CreatedBy = 3 },
                new ImportOrder { ImportOrderId = 5, InvoiceNumber = "IMP-20251001-001", OrderDate = new DateOnly(2025, 10, 1), ProviderId = 1, CreatedDate = new DateOnly(2025, 10, 1), Status = "Pending", CreatedBy = 2 },
                new ImportOrder { ImportOrderId = 6, InvoiceNumber = "IMP-20251005-002", OrderDate = new DateOnly(2025, 10, 5), ProviderId = 2, CreatedDate = new DateOnly(2025, 10, 5), Status = "Cancelled", CreatedBy = 3 },
                new ImportOrder { ImportOrderId = 7, InvoiceNumber = "IMP-20251102-001", OrderDate = new DateOnly(2025, 11, 2), ProviderId = 1, CreatedDate = new DateOnly(2025, 11, 2), Status = "Pending", CreatedBy = 2 },
                new ImportOrder { ImportOrderId = 8, InvoiceNumber = "IMP-20251103-001", OrderDate = new DateOnly(2025, 11, 3), ProviderId = 2, CreatedDate = new DateOnly(2025, 11, 3), Status = "Pending", CreatedBy = 3 },
                new ImportOrder { ImportOrderId = 9, InvoiceNumber = "IMP-20251104-001", OrderDate = new DateOnly(2025, 11, 4), ProviderId = 1, CreatedDate = new DateOnly(2025, 11, 4), Status = "Completed", CreatedBy = 2 },
                new ImportOrder { ImportOrderId = 10, InvoiceNumber = "IMP-20251105-002", OrderDate = new DateOnly(2025, 11, 5), ProviderId = 2, CreatedDate = new DateOnly(2025, 11, 5), Status = "Completed", CreatedBy = 3 },
                new ImportOrder { ImportOrderId = 11, InvoiceNumber = "IMP-20251106-001", OrderDate = new DateOnly(2025, 11, 6), ProviderId = 1, CreatedDate = new DateOnly(2025, 11, 6), Status = "Pending", CreatedBy = 2 },
                new ImportOrder { ImportOrderId = 12, InvoiceNumber = "IMP-20251107-001", OrderDate = new DateOnly(2025, 11, 7), ProviderId = 2, CreatedDate = new DateOnly(2025, 11, 7), Status = "Pending", CreatedBy = 3 }
            );

            modelBuilder.Entity<ImportDetail>().HasData(
                new ImportDetail { ImportDetailId = 5, ImportOrderId = 3, ProductId = 13, Quantity = 100, ImportPrice = 40000m },
                new ImportDetail { ImportDetailId = 6, ImportOrderId = 3, ProductId = 14, Quantity = 80, ImportPrice = 20000m },
                new ImportDetail { ImportDetailId = 7, ImportOrderId = 4, ProductId = 15, Quantity = 50, ImportPrice = 100000m },
                new ImportDetail { ImportDetailId = 8, ImportOrderId = 4, ProductId = 16, Quantity = 200, ImportPrice = 40000m },
                new ImportDetail { ImportDetailId = 9, ImportOrderId = 5, ProductId = 17, Quantity = 60, ImportPrice = 70000m },
                new ImportDetail { ImportDetailId = 10, ImportOrderId = 5, ProductId = 18, Quantity = 30, ImportPrice = 90000m },
                new ImportDetail { ImportDetailId = 11, ImportOrderId = 6, ProductId = 19, Quantity = 25, ImportPrice = 200000m },
                new ImportDetail { ImportDetailId = 12, ImportOrderId = 6, ProductId = 20, Quantity = 40, ImportPrice = 300000m },
                new ImportDetail { ImportDetailId = 13, ImportOrderId = 7, ProductId = 1, Quantity = 10, ImportPrice = 20000000m },
                new ImportDetail { ImportDetailId = 14, ImportOrderId = 8, ProductId = 2, Quantity = 15, ImportPrice = 2000000m },
                new ImportDetail { ImportDetailId = 15, ImportOrderId = 9, ProductId = 3, Quantity = 5, ImportPrice = 2500000m },
                new ImportDetail { ImportDetailId = 16, ImportOrderId = 9, ProductId = 13, Quantity = 20, ImportPrice = 40000m },
                new ImportDetail { ImportDetailId = 17, ImportOrderId = 10, ProductId = 14, Quantity = 30, ImportPrice = 20000m },
                new ImportDetail { ImportDetailId = 18, ImportOrderId = 11, ProductId = 15, Quantity = 8, ImportPrice = 100000m },
                new ImportDetail { ImportDetailId = 19, ImportOrderId = 12, ProductId = 16, Quantity = 60, ImportPrice = 40000m },
                new ImportDetail { ImportDetailId = 20, ImportOrderId = 12, ProductId = 17, Quantity = 12, ImportPrice = 70000m },
                new ImportDetail { ImportDetailId = 21, ImportOrderId = 7, ProductId = 18, Quantity = 5, ImportPrice = 90000m },
                new ImportDetail { ImportDetailId = 22, ImportOrderId = 8, ProductId = 19, Quantity = 3, ImportPrice = 200000m },
                new ImportDetail { ImportDetailId = 23, ImportOrderId = 10, ProductId = 20, Quantity = 10, ImportPrice = 300000m },
                new ImportDetail { ImportDetailId = 24, ImportOrderId = 11, ProductId = 4, Quantity = 2, ImportPrice = 12000000m }
            );

            // Bulk Export Orders (ExportOrderId 3..12) and ExportDetails (ExportDetailId 3..24)
            modelBuilder.Entity<ExportOrder>().HasData(
                new ExportOrder { ExportOrderId = 3, InvoiceNumber = "EXP-20250910-001", OrderDate = new DateOnly(2025, 9, 10), CustomerId = 3, Currency = "VND", CreatedDate = new DateOnly(2025, 9, 10), Status = "Delivered", CreatedBy = 2 },
                new ExportOrder { ExportOrderId = 4, InvoiceNumber = "EXP-20250920-001", OrderDate = new DateOnly(2025, 9, 20), CustomerId = 4, Currency = "USD", CreatedDate = new DateOnly(2025, 9, 20), Status = "Cancelled", CreatedBy = 3 },
                new ExportOrder { ExportOrderId = 5, InvoiceNumber = "EXP-20251001-001", OrderDate = new DateOnly(2025, 10, 1), CustomerId = 5, Currency = "VND", CreatedDate = new DateOnly(2025, 10, 1), Status = "Pending", CreatedBy = 2 },
                new ExportOrder { ExportOrderId = 6, InvoiceNumber = "EXP-20251010-001", OrderDate = new DateOnly(2025, 10, 10), CustomerId = 3, Currency = "VND", CreatedDate = new DateOnly(2025, 10, 10), Status = "Shipped", CreatedBy = 3 },
                new ExportOrder { ExportOrderId = 7, InvoiceNumber = "EXP-20251102-001", OrderDate = new DateOnly(2025, 11, 2), CustomerId = 4, Currency = "USD", CreatedDate = new DateOnly(2025, 11, 2), Status = "Pending", CreatedBy = 2 },
                new ExportOrder { ExportOrderId = 8, InvoiceNumber = "EXP-20251103-001", OrderDate = new DateOnly(2025, 11, 3), CustomerId = 5, Currency = "VND", CreatedDate = new DateOnly(2025, 11, 3), Status = "Pending", CreatedBy = 3 },
                new ExportOrder { ExportOrderId = 9, InvoiceNumber = "EXP-20251104-001", OrderDate = new DateOnly(2025, 11, 4), CustomerId = 3, Currency = "VND", CreatedDate = new DateOnly(2025, 11, 4), Status = "Delivered", CreatedBy = 2 },
                new ExportOrder { ExportOrderId = 10, InvoiceNumber = "EXP-20251105-002", OrderDate = new DateOnly(2025, 11, 5), CustomerId = 4, Currency = "USD", CreatedDate = new DateOnly(2025, 11, 5), Status = "Shipped", CreatedBy = 3 },
                new ExportOrder { ExportOrderId = 11, InvoiceNumber = "EXP-20251106-001", OrderDate = new DateOnly(2025, 11, 6), CustomerId = 5, Currency = "VND", CreatedDate = new DateOnly(2025, 11, 6), Status = "Pending", CreatedBy = 2 },
                new ExportOrder { ExportOrderId = 12, InvoiceNumber = "EXP-20251107-001", OrderDate = new DateOnly(2025, 11, 7), CustomerId = 3, Currency = "VND", CreatedDate = new DateOnly(2025, 11, 7), Status = "Pending", CreatedBy = 3 }
            );

            modelBuilder.Entity<ExportDetail>().HasData(
                new ExportDetail { ExportDetailId = 3, ExportOrderId = 3, ProductId = 13, Quantity = 20, TotalPrice = 50000m * 20 },
                new ExportDetail { ExportDetailId = 4, ExportOrderId = 3, ProductId = 14, Quantity = 10, TotalPrice = 30000m * 10 },
                new ExportDetail { ExportDetailId = 5, ExportOrderId = 4, ProductId = 15, Quantity = 5, TotalPrice = 150000m * 5 },
                new ExportDetail { ExportDetailId = 6, ExportOrderId = 5, ProductId = 16, Quantity = 30, TotalPrice = 80000m * 30 },
                new ExportDetail { ExportDetailId = 7, ExportOrderId = 6, ProductId = 1, Quantity = 2, TotalPrice = 25000000m * 2 },
                new ExportDetail { ExportDetailId = 8, ExportOrderId = 7, ProductId = 2, Quantity = 10, TotalPrice = 2500000m * 10 },
                new ExportDetail { ExportDetailId = 9, ExportOrderId = 8, ProductId = 3, Quantity = 3, TotalPrice = 3000000m * 3 },
                new ExportDetail { ExportDetailId = 10, ExportOrderId = 9, ProductId = 4, Quantity = 1, TotalPrice = 15000000m },
                new ExportDetail { ExportDetailId = 11, ExportOrderId = 10, ProductId = 5, Quantity = 2, TotalPrice = 8000000m * 2 },
                new ExportDetail { ExportDetailId = 12, ExportOrderId = 11, ProductId = 6, Quantity = 4, TotalPrice = 5000000m * 4 },
                new ExportDetail { ExportDetailId = 13, ExportOrderId = 12, ProductId = 7, Quantity = 2, TotalPrice = 30000000m * 2 },
                new ExportDetail { ExportDetailId = 14, ExportOrderId = 5, ProductId = 13, Quantity = 15, TotalPrice = 50000m * 15 },
                new ExportDetail { ExportDetailId = 15, ExportOrderId = 6, ProductId = 14, Quantity = 8, TotalPrice = 30000m * 8 },
                new ExportDetail { ExportDetailId = 16, ExportOrderId = 7, ProductId = 15, Quantity = 6, TotalPrice = 150000m * 6 },
                new ExportDetail { ExportDetailId = 17, ExportOrderId = 8, ProductId = 16, Quantity = 20, TotalPrice = 80000m * 20 },
                new ExportDetail { ExportDetailId = 18, ExportOrderId = 9, ProductId = 17, Quantity = 5, TotalPrice = 90000m * 5 },
                new ExportDetail { ExportDetailId = 19, ExportOrderId = 10, ProductId = 18, Quantity = 3, TotalPrice = 110000m * 3 },
                new ExportDetail { ExportDetailId = 20, ExportOrderId = 11, ProductId = 19, Quantity = 2, TotalPrice = 350000m * 2 },
                new ExportDetail { ExportDetailId = 21, ExportOrderId = 12, ProductId = 20, Quantity = 4, TotalPrice = 450000m * 4 },
                new ExportDetail { ExportDetailId = 22, ExportOrderId = 3, ProductId = 1, Quantity = 1, TotalPrice = 25000000m },
                new ExportDetail { ExportDetailId = 23, ExportOrderId = 4, ProductId = 2, Quantity = 2, TotalPrice = 2500000m * 2 },
                new ExportDetail { ExportDetailId = 24, ExportOrderId = 5, ProductId = 3, Quantity = 3, TotalPrice = 3000000m * 3 }
            );

            // Additional Return Orders (ReturnOrderId 3..8) and details (ReturnDetailId 3..8)
            modelBuilder.Entity<ReturnOrder>().HasData(
                new ReturnOrder { ReturnOrderId = 3, ExportOrderId = 3, CheckedBy = 3, ReviewedBy = 2, CheckInTime = new DateTime(2025, 9, 12, 11, 0, 0), Status = "Processed", Note = "Minor damage" },
                new ReturnOrder { ReturnOrderId = 4, ExportOrderId = 4, CheckedBy = 3, ReviewedBy = 2, CheckInTime = new DateTime(2025, 9, 21, 14, 0, 0), Status = "Cancelled", Note = "Customer withdrew" },
                new ReturnOrder { ReturnOrderId = 5, ExportOrderId = 5, CheckedBy = 2, ReviewedBy = 3, CheckInTime = new DateTime(2025, 10, 3, 9, 30, 0), Status = "Pending", Note = "Quality issue" },
                new ReturnOrder { ReturnOrderId = 6, ExportOrderId = 6, CheckedBy = 2, ReviewedBy = 3, CheckInTime = new DateTime(2025, 10, 11, 10, 0, 0), Status = "Processed", Note = "Expired item" },
                new ReturnOrder { ReturnOrderId = 7, ExportOrderId = 7, CheckedBy = 3, ReviewedBy = 2, CheckInTime = new DateTime(2025, 11, 4, 15, 0, 0), Status = "Pending", Note = "Wrong item" },
                new ReturnOrder { ReturnOrderId = 8, ExportOrderId = 8, CheckedBy = 3, ReviewedBy = 2, CheckInTime = new DateTime(2025, 11, 5, 16, 30, 0), Status = "Processed", Note = "Customer request" }
            );

            modelBuilder.Entity<ReturnOrderDetail>().HasData(
                new ReturnOrderDetail { ReturnDetailId = 3, ReturnOrderId = 3, ProductId = 13, Quantity = 2, Note = "Crushed", ReasonId = 1, ActionId = null, LocationId = 13 },
                new ReturnOrderDetail { ReturnDetailId = 4, ReturnOrderId = 4, ProductId = 14, Quantity = 1, Note = "Not needed", ReasonId = 6, ActionId = null, LocationId = 14 },
                new ReturnOrderDetail { ReturnDetailId = 5, ReturnOrderId = 5, ProductId = 15, Quantity = 1, Note = "Defect", ReasonId = 2, ActionId = null, LocationId = 15 },
                new ReturnOrderDetail { ReturnDetailId = 6, ReturnOrderId = 6, ProductId = 17, Quantity = 4, Note = "Expired soon", ReasonId = 4, ActionId = null, LocationId = 17 },
                new ReturnOrderDetail { ReturnDetailId = 7, ReturnOrderId = 7, ProductId = 16, Quantity = 2, Note = "Wrong SKU", ReasonId = 3, ActionId = null, LocationId = 16 },
                new ReturnOrderDetail { ReturnDetailId = 8, ReturnOrderId = 8, ProductId = 18, Quantity = 1, Note = "Customer request", ReasonId = 6, ActionId = null, LocationId = 18 }
            );
        }

        /// <summary>
        /// Apply data seeding to the database context
        /// </summary>
        public static void ApplyDataSeeding(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartWarehouseDbContext>();
            
            // Ensure database is created and migrations are applied
            context.Database.Migrate();
            
            // Additional runtime seeding can be added here if needed
            // Example: Check if data exists, if not, add it
            if (!context.Users.Any())
            {
                Console.WriteLine("[Data Seeding] No users found. Seeding will be applied via migrations.");
            }
        }
    }
}

