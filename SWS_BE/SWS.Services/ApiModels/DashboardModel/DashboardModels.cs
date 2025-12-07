using System;
using System.Collections.Generic;

namespace SWS.Services.ApiModels.DashboardModel
{
    // ============================================
    // 1. TÀI CHÍNH & HIỆU QUẢ (Giám đốc, Kế toán trưởng)
    // ============================================

    /// <summary>
    /// Phân tích Xu hướng theo thời gian - Thấy "mùa vụ" kinh doanh
    /// </summary>
    public class TrendAnalysisResponse
    {
        public DateOnly Date { get; set; }
        public decimal ImportValue { get; set; }  // Giá trị nhập kho
        public decimal ExportValue { get; set; }  // Giá trị xuất kho
        public decimal StockValue { get; set; }   // Giá trị tồn kho
        public int ImportQuantity { get; set; }   // Số lượng nhập
        public int ExportQuantity { get; set; }   // Số lượng xuất
        public int StockQuantity { get; set; }    // Số lượng tồn
    }

    /// <summary>
    /// So sánh Tăng trưởng theo kỳ - Đánh giá hiệu quả sử dụng vốn
    /// </summary>
    public class PeriodComparisonResponse
    {
        public string PeriodName { get; set; } = string.Empty;  // Tên kỳ (Tháng 1/2024, Q1/2024...)
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        
        // Số dư đầu kỳ
        public decimal OpeningStockValue { get; set; }
        public int OpeningStockQuantity { get; set; }
        
        // Số dư cuối kỳ
        public decimal ClosingStockValue { get; set; }
        public int ClosingStockQuantity { get; set; }
        
        // Tăng trưởng
        public decimal ValueGrowthPercent { get; set; }      // % tăng trưởng giá trị
        public decimal QuantityGrowthPercent { get; set; }   // % tăng trưởng số lượng
        
        // Doanh thu trong kỳ
        public decimal Revenue { get; set; }
        
        // Cảnh báo: Tồn kho tăng nhưng doanh thu không tăng tương ứng
        public bool IsCapitalStagnation { get; set; }
        public string Warning { get; set; } = string.Empty;
    }

    /// <summary>
    /// Tách biệt Giá trị vs Số lượng - Phục vụ Kế toán và Thủ kho
    /// </summary>
    public class ValueVsQuantityResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        
        // Cho Kế toán - Cân đối dòng tiền
        public decimal UnitCost { get; set; }           // Đơn giá vốn
        public decimal TotalValue { get; set; }         // Tổng giá trị
        public decimal AverageCost { get; set; }        // Giá vốn trung bình
        
        // Cho Thủ kho - Cân đối diện tích kho
        public int TotalQuantity { get; set; }          // Số lượng tồn
        public string LocationInfo { get; set; } = string.Empty;  // Vị trí kho
        public decimal OccupiedSpace { get; set; }      // Diện tích chiếm dụng (m²)
    }

    // ============================================
    // 2. VẬN HÀNH & TỐI ƯU (Thủ kho, Quản lý kho)
    // ============================================

    /// <summary>
    /// Phân tích Cấu trúc (Top 10) - Quy luật 80/20
    /// </summary>
    public class Top10StructureAnalysisResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        
        public decimal TotalValue { get; set; }         // Tổng giá trị tồn
        public int TotalQuantity { get; set; }          // Tổng số lượng tồn
        
        public decimal ValuePercentage { get; set; }    // % giá trị so với tổng kho
        public decimal CumulativePercentage { get; set; } // % tích lũy
        
        public string Category { get; set; } = string.Empty;  // A, B, C (theo ABC Analysis)
        public string ManagementPriority { get; set; } = string.Empty;  // Ưu tiên quản lý
    }

    /// <summary>
    /// Phân tích theo Kho (Vị trí) - Cân bằng hàng hóa giữa các kho
    /// </summary>
    public class WarehouseBalanceResponse
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string ShelfId { get; set; } = string.Empty;
        
        public int TotalProducts { get; set; }          // Số loại sản phẩm
        public int TotalQuantity { get; set; }          // Tổng số lượng
        public decimal TotalValue { get; set; }         // Tổng giá trị
        
        public decimal CapacityUsed { get; set; }       // % công suất sử dụng
        public bool IsOverloaded { get; set; }          // Cảnh báo quá tải
        public bool IsUnderUtilized { get; set; }       // Cảnh báo dùng dưới mức
        
        public List<TransferSuggestion> TransferSuggestions { get; set; } = new();
    }

    public class TransferSuggestion
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public int SuggestedQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal EstimatedCostSaving { get; set; }
    }

    // ============================================
    // 3. QUẢN TRỊ RỦI RO (Kiểm soát nội bộ, Mua hàng)
    // ============================================

    /// <summary>
    /// Cảnh báo Tồn tối thiểu - Gợi ý đặt hàng
    /// </summary>
    public class MinimumStockAlertResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        
        public int CurrentStock { get; set; }           // Tồn kho hiện tại
        public int MinimumStock { get; set; }           // Định mức tồn tối thiểu
        public int ReorderPoint { get; set; }           // Điểm đặt hàng lại
        
        public int ShortageQuantity { get; set; }       // Số lượng thiếu hụt
        public string AlertLevel { get; set; } = string.Empty;  // Critical, Warning, Info
        
        public int SuggestedOrderQuantity { get; set; } // Số lượng đề xuất đặt hàng
        public decimal EstimatedCost { get; set; }      // Chi phí ước tính
        
        public int LeadTimeDays { get; set; }           // Thời gian giao hàng dự kiến
        public DateOnly SuggestedOrderDate { get; set; } // Ngày đề xuất đặt hàng
        
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Phân tích Hạn sử dụng - Nguyên tắc FEFO (First Expired First Out)
    /// </summary>
    public class ExpiryDateAnalysisResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;  // Số Lô
        
        public DateOnly ExpiryDate { get; set; }        // Hạn sử dụng
        public int DaysUntilExpiry { get; set; }        // Số ngày đến hạn
        
        public int Quantity { get; set; }               // Số lượng trong lô
        public decimal Value { get; set; }              // Giá trị lô hàng
        
        public string LocationInfo { get; set; } = string.Empty;
        
        public string ExpiryStatus { get; set; } = string.Empty;  // NearExpiry, Critical, Expired
        public string FEFOPriority { get; set; } = string.Empty;  // Ưu tiên xuất (High, Medium, Low)
        
        public List<string> ActionSuggestions { get; set; } = new();  // Khuyến nghị hành động
    }

    /// <summary>
    /// Báo cáo Hàng quá hạn (Dead Stock) - Tính thiệt hại & truy trách nhiệm
    /// </summary>
    public class DeadStockReportResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        
        public DateOnly ExpiryDate { get; set; }
        public int DaysOverdue { get; set; }            // Số ngày quá hạn
        
        public int Quantity { get; set; }
        public decimal OriginalValue { get; set; }      // Giá trị gốc
        public decimal LiquidationValue { get; set; }   // Giá trị thanh lý ước tính
        public decimal TotalLoss { get; set; }          // Tổng thiệt hại
        
        public string LocationInfo { get; set; } = string.Empty;
        
        // Truy trách nhiệm
        public int? ResponsibleUserId { get; set; }
        public string ResponsibleUserName { get; set; } = string.Empty;
        public string ResponsibleRole { get; set; } = string.Empty;
        
        public DateOnly ImportDate { get; set; }        // Ngày nhập kho
        public int ImportOrderId { get; set; }
        
        public string DisposalStatus { get; set; } = string.Empty;  // Pending, InProgress, Completed
        public string DisposalMethod { get; set; } = string.Empty;  // Return to Supplier, Charity, Destroy
    }

    /// <summary>
    /// Tổng quan Dashboard - Tổng hợp các chỉ số chính
    /// </summary>
    public class DashboardOverviewResponse
    {
        // Tài chính
        public decimal TotalStockValue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal ValueGrowthRate { get; set; }
        
        // Vận hành
        public int TotalProducts { get; set; }
        public int TotalLocations { get; set; }
        public decimal AverageCapacityUsage { get; set; }
        
        // Rủi ro
        public int LowStockAlertCount { get; set; }
        public int NearExpiryCount { get; set; }
        public int ExpiredStockCount { get; set; }
        public decimal TotalPotentialLoss { get; set; }
    }
}
