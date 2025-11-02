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

