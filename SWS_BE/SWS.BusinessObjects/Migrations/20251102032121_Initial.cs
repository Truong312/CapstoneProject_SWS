using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWS.BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessPartner",
                columns: table => new
                {
                    PartnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Business__39FD6332B4BB7E70", x => x.PartnerID);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShelfID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ColumnNumber = table.Column<int>(type: "int", nullable: true),
                    RowNumber = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsFull = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__E7FEA477D7617922", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiredDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReceivedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PurchasedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReorderPoint = table.Column<int>(type: "int", nullable: true, defaultValue: 10),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__B40CC6ED54146263", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "ReturnAction",
                columns: table => new
                {
                    ActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnAc__FFE3F4B9CF7781B0", x => x.ActionID);
                });

            migrationBuilder.CreateTable(
                name: "ReturnReason",
                columns: table => new
                {
                    ReasonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReasonCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnRe__A4F8C0C7333F6FD6", x => x.ReasonID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CCAC9EDB8B52", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false),
                    AllocatedQuantity = table.Column<int>(type: "int", nullable: false),
                    LocationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__F5FDE6D309535C66", x => x.InventoryID);
                    table.ForeignKey(
                        name: "FK__Inventory__Locat__44FF419A",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID");
                    table.ForeignKey(
                        name: "FK__Inventory__Produ__440B1D61",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ActionLog",
                columns: table => new
                {
                    ActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ActionLo__FFE3F4B99CCDB16A", x => x.ActionID);
                    table.ForeignKey(
                        name: "FK__ActionLog__UserI__6E01572D",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ExportOrder",
                columns: table => new
                {
                    ExportOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    ShippedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ShippedAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExportOr__618D04DEE3154923", x => x.ExportOrderID);
                    table.ForeignKey(
                        name: "FK__ExportOrd__Creat__52593CB8",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ExportOrd__Custo__5165187F",
                        column: x => x.CustomerId,
                        principalTable: "BusinessPartner",
                        principalColumn: "PartnerID");
                });

            migrationBuilder.CreateTable(
                name: "ImportOrder",
                columns: table => new
                {
                    ImportOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    ArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImportOr__EAFFE315B675763C", x => x.ImportOrderID);
                    table.ForeignKey(
                        name: "FK__ImportOrd__Creat__49C3F6B7",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ImportOrd__Provi__48CFD27E",
                        column: x => x.ProviderId,
                        principalTable: "BusinessPartner",
                        principalColumn: "PartnerID");
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    QuantityChanged = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__55433A4B28FD9381", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK__Transacti__Creat__6A30C649",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Transacti__Produ__693CA210",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ExportDetail",
                columns: table => new
                {
                    ExportDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportOrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExportDe__07C903593B71807C", x => x.ExportDetailID);
                    table.ForeignKey(
                        name: "FK__ExportDet__Expor__5535A963",
                        column: x => x.ExportOrderID,
                        principalTable: "ExportOrder",
                        principalColumn: "ExportOrderID");
                    table.ForeignKey(
                        name: "FK__ExportDet__Produ__5629CD9C",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ReturnOrder",
                columns: table => new
                {
                    ReturnOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportOrderID = table.Column<int>(type: "int", nullable: true),
                    CheckedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnOr__4DBF556330CEAA6C", x => x.ReturnOrderID);
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Check__5DCAEF64",
                        column: x => x.CheckedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Expor__5CD6CB2B",
                        column: x => x.ExportOrderID,
                        principalTable: "ExportOrder",
                        principalColumn: "ExportOrderID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Revie__5EBF139D",
                        column: x => x.ReviewedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ImportDetail",
                columns: table => new
                {
                    ImportDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportOrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImportPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ImportDe__CDFBBA51B46C934D", x => x.ImportDetailID);
                    table.ForeignKey(
                        name: "FK__ImportDet__Impor__4CA06362",
                        column: x => x.ImportOrderID,
                        principalTable: "ImportOrder",
                        principalColumn: "ImportOrderID");
                    table.ForeignKey(
                        name: "FK__ImportDet__Produ__4D94879B",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ReturnOrderDetail",
                columns: table => new
                {
                    ReturnDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnOrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReasonID = table.Column<int>(type: "int", nullable: true),
                    ActionID = table.Column<int>(type: "int", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnOr__8B89C9EA015F21E8", x => x.ReturnDetailID);
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Actio__6477ECF3",
                        column: x => x.ActionID,
                        principalTable: "ReturnAction",
                        principalColumn: "ActionID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Locat__656C112C",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Produ__628FA481",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Reaso__6383C8BA",
                        column: x => x.ReasonID,
                        principalTable: "ReturnReason",
                        principalColumn: "ReasonID");
                    table.ForeignKey(
                        name: "FK__ReturnOrd__Retur__619B8048",
                        column: x => x.ReturnOrderID,
                        principalTable: "ReturnOrder",
                        principalColumn: "ReturnOrderID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_UserID",
                table: "ActionLog",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetail_ExportOrderID",
                table: "ExportDetail",
                column: "ExportOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetail_ProductID",
                table: "ExportDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ExportOrder_CreatedBy",
                table: "ExportOrder",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExportOrder_CustomerId",
                table: "ExportOrder",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_ImportOrderID",
                table: "ImportDetail",
                column: "ImportOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_ProductID",
                table: "ImportDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrder_CreatedBy",
                table: "ImportOrder",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrder_ProviderId",
                table: "ImportOrder",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_LocationID",
                table: "Inventory",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductID",
                table: "Inventory",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrder_CheckedBy",
                table: "ReturnOrder",
                column: "CheckedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrder_ExportOrderID",
                table: "ReturnOrder",
                column: "ExportOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrder_ReviewedBy",
                table: "ReturnOrder",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetail_ActionID",
                table: "ReturnOrderDetail",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetail_LocationID",
                table: "ReturnOrderDetail",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetail_ProductID",
                table: "ReturnOrderDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetail_ReasonID",
                table: "ReturnOrderDetail",
                column: "ReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetail_ReturnOrderID",
                table: "ReturnOrderDetail",
                column: "ReturnOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_CreatedBy",
                table: "TransactionLog",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_ProductID",
                table: "TransactionLog",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "UQ__User__A9D10534DB2DDA05",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLog");

            migrationBuilder.DropTable(
                name: "ExportDetail");

            migrationBuilder.DropTable(
                name: "ImportDetail");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "ReturnOrderDetail");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropTable(
                name: "ImportOrder");

            migrationBuilder.DropTable(
                name: "ReturnAction");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "ReturnReason");

            migrationBuilder.DropTable(
                name: "ReturnOrder");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ExportOrder");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "BusinessPartner");
        }
    }
}
