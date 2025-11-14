using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SWS.BusinessObjects.Models;

public partial class SmartWarehouseDbContext : DbContext
{
    public SmartWarehouseDbContext()
    {
    }

    public SmartWarehouseDbContext(DbContextOptions<SmartWarehouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionLog> ActionLogs { get; set; }

    public virtual DbSet<BusinessPartner> BusinessPartners { get; set; }

    public virtual DbSet<CycleCount> CycleCounts { get; set; }

    public virtual DbSet<CycleCountDetail> CycleCountDetails { get; set; }

    public virtual DbSet<ExportDetail> ExportDetails { get; set; }

    public virtual DbSet<ExportOrder> ExportOrders { get; set; }

    public virtual DbSet<ImportDetail> ImportDetails { get; set; }

    public virtual DbSet<ImportOrder> ImportOrders { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ReturnAction> ReturnActions { get; set; }

    public virtual DbSet<ReturnOrder> ReturnOrders { get; set; }

    public virtual DbSet<ReturnOrderDetail> ReturnOrderDetails { get; set; }

    public virtual DbSet<ReturnReason> ReturnReasons { get; set; }

    public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionLog>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__ActionLo__FFE3F4B95252482B");

            entity.ToTable("ActionLog");

            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.ActionType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ActionLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActionLog__UserI__6E01572D");
        });

        modelBuilder.Entity<BusinessPartner>(entity =>
        {
            entity.HasKey(e => e.PartnerId).HasName("PK__Business__39FD63324D0693B4");

            entity.ToTable("BusinessPartner");

            entity.Property(e => e.PartnerId).HasColumnName("PartnerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<CycleCount>(entity =>
        {
            entity.HasKey(e => e.CycleCountId).HasName("PK__CycleCou__A4189B51C1D19CDB");

            entity.ToTable("CycleCount");

            entity.Property(e => e.CycleCountId).HasColumnName("CycleCountID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CycleName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<CycleCountDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__CycleCou__135C314D899B9755");

            entity.ToTable("CycleCountDetail");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.CheckedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CycleCountId).HasColumnName("CycleCountID");
            entity.Property(e => e.Difference).HasComputedColumnSql("([CountedQuantity]-[SystemQuantity])", false);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.CheckedByNavigation).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.CheckedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CycleCoun__Check__09A971A2");

            entity.HasOne(d => d.CycleCount).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.CycleCountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CycleCoun__Cycle__07C12930");

            entity.HasOne(d => d.Product).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CycleCoun__Produ__08B54D69");
        });

        modelBuilder.Entity<ExportDetail>(entity =>
        {
            entity.HasKey(e => e.ExportDetailId).HasName("PK__ExportDe__07C903598CFEC015");

            entity.ToTable("ExportDetail");

            entity.Property(e => e.ExportDetailId).HasColumnName("ExportDetailID");
            entity.Property(e => e.ExportOrderId).HasColumnName("ExportOrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ExportOrder).WithMany(p => p.ExportDetails)
                .HasForeignKey(d => d.ExportOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportDet__Expor__5535A963");

            entity.HasOne(d => d.Product).WithMany(p => p.ExportDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportDet__Produ__5629CD9C");
        });

        modelBuilder.Entity<ExportOrder>(entity =>
        {
            entity.HasKey(e => e.ExportOrderId).HasName("PK__ExportOr__618D04DEB462B526");

            entity.ToTable("ExportOrder");

            entity.Property(e => e.ExportOrderId).HasColumnName("ExportOrderID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Currency).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.ShippedAddress).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TotalPayment).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExportOrders)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ExportOrd__Creat__52593CB8");

            entity.HasOne(d => d.Customer).WithMany(p => p.ExportOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportOrd__Custo__5165187F");
        });

        modelBuilder.Entity<ImportDetail>(entity =>
        {
            entity.HasKey(e => e.ImportDetailId).HasName("PK__ImportDe__CDFBBA51482A60DC");

            entity.ToTable("ImportDetail");

            entity.Property(e => e.ImportDetailId).HasColumnName("ImportDetailID");
            entity.Property(e => e.ImportOrderId).HasColumnName("ImportOrderID");
            entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.ImportOrder).WithMany(p => p.ImportDetails)
                .HasForeignKey(d => d.ImportOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportDet__Impor__4CA06362");

            entity.HasOne(d => d.Product).WithMany(p => p.ImportDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportDet__Produ__4D94879B");
        });

        modelBuilder.Entity<ImportOrder>(entity =>
        {
            entity.HasKey(e => e.ImportOrderId).HasName("PK__ImportOr__EAFFE3151D8202F1");

            entity.ToTable("ImportOrder");

            entity.Property(e => e.ImportOrderId).HasColumnName("ImportOrderID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ImportOrders)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ImportOrd__Creat__49C3F6B7");

            entity.HasOne(d => d.Provider).WithMany(p => p.ImportOrders)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportOrd__Provi__48CFD27E");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D3CF9EDED3");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Location).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Locat__44FF419A");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__440B1D61");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA477F338D051");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.IsFull).HasDefaultValue(false);
            entity.Property(e => e.ShelfId)
                .HasMaxLength(50)
                .HasColumnName("ShelfID");
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED4329EB44");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PurchasedPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReorderPoint).HasDefaultValue(10);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ReturnAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__ReturnAc__FFE3F4B957EB09FD");

            entity.ToTable("ReturnAction");

            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<ReturnOrder>(entity =>
        {
            entity.HasKey(e => e.ReturnOrderId).HasName("PK__ReturnOr__4DBF556378E06DEC");

            entity.ToTable("ReturnOrder");

            entity.Property(e => e.ReturnOrderId).HasColumnName("ReturnOrderID");
            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.ExportOrderId).HasColumnName("ExportOrderID");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CheckedByNavigation).WithMany(p => p.ReturnOrderCheckedByNavigations)
                .HasForeignKey(d => d.CheckedBy)
                .HasConstraintName("FK__ReturnOrd__Check__5DCAEF64");

            entity.HasOne(d => d.ExportOrder).WithMany(p => p.ReturnOrders)
                .HasForeignKey(d => d.ExportOrderId)
                .HasConstraintName("FK__ReturnOrd__Expor__5CD6CB2B");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ReturnOrderReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__ReturnOrd__Revie__5EBF139D");
        });

        modelBuilder.Entity<ReturnOrderDetail>(entity =>
        {
            entity.HasKey(e => e.ReturnDetailId).HasName("PK__ReturnOr__8B89C9EAC4864254");

            entity.ToTable("ReturnOrderDetail");

            entity.Property(e => e.ReturnDetailId).HasColumnName("ReturnDetailID");
            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.ReturnOrderId).HasColumnName("ReturnOrderID");

            entity.HasOne(d => d.Action).WithMany(p => p.ReturnOrderDetails)
                .HasForeignKey(d => d.ActionId)
                .HasConstraintName("FK__ReturnOrd__Actio__6477ECF3");

            entity.HasOne(d => d.Location).WithMany(p => p.ReturnOrderDetails)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__ReturnOrd__Locat__656C112C");

            entity.HasOne(d => d.Product).WithMany(p => p.ReturnOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReturnOrd__Produ__628FA481");

            entity.HasOne(d => d.Reason).WithMany(p => p.ReturnOrderDetails)
                .HasForeignKey(d => d.ReasonId)
                .HasConstraintName("FK__ReturnOrd__Reaso__6383C8BA");

            entity.HasOne(d => d.ReturnOrder).WithMany(p => p.ReturnOrderDetails)
                .HasForeignKey(d => d.ReturnOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReturnOrd__Retur__619B8048");
        });

        modelBuilder.Entity<ReturnReason>(entity =>
        {
            entity.HasKey(e => e.ReasonId).HasName("PK__ReturnRe__A4F8C0C7D482E53D");

            entity.ToTable("ReturnReason");

            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ReasonCode).HasMaxLength(50);
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B8F2EB147");

            entity.ToTable("TransactionLog");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Transacti__Creat__6A30C649");

            entity.HasOne(d => d.Product).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Transacti__Produ__693CA210");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC8EA782E8");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534577EB67D").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
