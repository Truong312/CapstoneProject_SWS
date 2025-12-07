using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.DashboardModel;

namespace SWS.Services.Services.DashboardServices
{
    public interface IDashboardService
    {
        // ============================================
        // 1. TÀI CHÍNH & HIỆU QUẢ (Giám đốc, Kế toán trưởng)
        // ============================================
        
        /// <summary>
        /// Phân tích Xu hướng theo thời gian - Nhìn thấy "mùa vụ" kinh doanh
        /// </summary>
        Task<ResultModel<List<TrendAnalysisResponse>>> GetTrendAnalysisAsync(DateOnly startDate, DateOnly endDate);
        
        /// <summary>
        /// So sánh Tăng trưởng kỳ - Đánh giá hiệu quả sử dụng vốn
        /// </summary>
        Task<ResultModel<List<PeriodComparisonResponse>>> GetPeriodComparisonAsync(string periodType, int count = 6);
        
        /// <summary>
        /// Tách biệt Giá trị vs Số lượng - Phục vụ Kế toán và Thủ kho
        /// </summary>
        Task<ResultModel<List<ValueVsQuantityResponse>>> GetValueVsQuantityAnalysisAsync();

        // ============================================
        // 2. VẬN HÀNH & TỐI ƯU (Thủ kho, Quản lý kho)
        // ============================================
        
        /// <summary>
        /// Phân tích Cấu trúc (Top 10) - Áp dụng quy luật 80/20
        /// </summary>
        Task<ResultModel<List<Top10StructureAnalysisResponse>>> GetTop10StructureAnalysisAsync();
        
        /// <summary>
        /// Phân tích theo Kho (Vị trí) - Cân bằng hàng hóa giữa các chi nhánh/kho bãi
        /// </summary>
        Task<ResultModel<List<WarehouseBalanceResponse>>> GetWarehouseBalanceAnalysisAsync();

        // ============================================
        // 3. QUẢN TRỊ RỦI RO (Kiểm soát nội bộ, Mua hàng)
        // ============================================
        
        /// <summary>
        /// Cảnh báo Tồn tối thiểu - Gợi ý đặt hàng chính xác
        /// </summary>
        Task<ResultModel<List<MinimumStockAlertResponse>>> GetMinimumStockAlertsAsync();
        
        /// <summary>
        /// Phân tích Hạn sử dụng - Áp dụng nguyên tắc FEFO
        /// </summary>
        Task<ResultModel<List<ExpiryDateAnalysisResponse>>> GetExpiryDateAnalysisAsync(int daysThreshold = 30);
        
        /// <summary>
        /// Báo cáo Hàng quá hạn (Dead Stock) - Tính thiệt hại & truy trách nhiệm
        /// </summary>
        Task<ResultModel<List<DeadStockReportResponse>>> GetDeadStockReportAsync();

        // ============================================
        // TỔNG QUAN
        // ============================================
        
        /// <summary>
        /// Dashboard tổng quan - Tất cả chỉ số chính
        /// </summary>
        Task<ResultModel<DashboardOverviewResponse>> GetDashboardOverviewAsync();
    }
}
