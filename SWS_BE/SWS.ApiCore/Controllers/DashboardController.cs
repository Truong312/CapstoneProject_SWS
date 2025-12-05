using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.Services.DashboardServices;
using System;
using System.Threading.Tasks;

namespace SWS.ApiCore.Controllers
{
    /// <summary>
    /// API Dashboard - Thống kê theo 3 nhóm chức năng chính
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ============================================
        // 1. TÀI CHÍNH & HIỆU QUẢ (Giám đốc, Kế toán trưởng)
        // ============================================

        /// <summary>
        /// [TÀI CHÍNH] Phân tích Xu hướng theo thời gian - Nhìn thấy "mùa vụ" kinh doanh
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (format: yyyy-MM-dd)</param>
        /// <param name="endDate">Ngày kết thúc (format: yyyy-MM-dd)</param>
        /// <returns>Xu hướng nhập/xuất/tồn kho theo thời gian</returns>
        [HttpGet("finance/trend-analysis")]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> GetTrendAnalysis(
            [FromQuery] DateOnly startDate, 
            [FromQuery] DateOnly endDate)
        {
            var result = await _dashboardService.GetTrendAnalysisAsync(startDate, endDate);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        /// <summary>
        /// [TÀI CHÍNH] So sánh Tăng trưởng theo kỳ - Đánh giá hiệu quả sử dụng vốn
        /// </summary>
        /// <param name="periodType">Loại kỳ: "month" hoặc "quarter"</param>
        /// <param name="count">Số kỳ cần so sánh (mặc định 6)</param>
        /// <returns>So sánh tăng trưởng và cảnh báo ứ đọng vốn</returns>
        [HttpGet("finance/period-comparison")]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> GetPeriodComparison(
            [FromQuery] string periodType = "month", 
            [FromQuery] int count = 6)
        {
            var result = await _dashboardService.GetPeriodComparisonAsync(periodType, count);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        /// <summary>
        /// [TÀI CHÍNH] Tách biệt Giá trị vs Số lượng - Phục vụ Kế toán và Thủ kho
        /// </summary>
        /// <returns>Phân tích giá trị (cho KT) và số lượng (cho Thủ kho)</returns>
        [HttpGet("finance/value-vs-quantity")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetValueVsQuantityAnalysis()
        {
            var result = await _dashboardService.GetValueVsQuantityAnalysisAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        // ============================================
        // 2. VẬN HÀNH & TỐI ƯU (Thủ kho, Quản lý kho)
        // ============================================

        /// <summary>
        /// [VẬN HÀNH] Phân tích Cấu trúc Top 10 - Áp dụng quy luật 80/20
        /// </summary>
        /// <returns>Top 10 sản phẩm chiếm giá trị cao nhất với phân loại ABC</returns>
        [HttpGet("operations/top10-structure")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetTop10StructureAnalysis()
        {
            var result = await _dashboardService.GetTop10StructureAnalysisAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        /// <summary>
        /// [VẬN HÀNH] Phân tích theo Kho - Cân bằng hàng hóa giữa các kho
        /// </summary>
        /// <returns>Phân tích công suất và đề xuất điều chuyển hàng</returns>
        [HttpGet("operations/warehouse-balance")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetWarehouseBalanceAnalysis()
        {
            var result = await _dashboardService.GetWarehouseBalanceAnalysisAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        // ============================================
        // 3. QUẢN TRỊ RỦI RO (Kiểm soát nội bộ, Mua hàng)
        // ============================================

        /// <summary>
        /// [RỦI RO] Cảnh báo Tồn tối thiểu - Gợi ý đặt hàng chính xác
        /// </summary>
        /// <returns>Danh sách cảnh báo thiếu hàng và gợi ý đặt hàng</returns>
        [HttpGet("risk/minimum-stock-alerts")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetMinimumStockAlerts()
        {
            var result = await _dashboardService.GetMinimumStockAlertsAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        /// <summary>
        /// [RỦI RO] Phân tích Hạn sử dụng - Nguyên tắc FEFO (First Expired First Out)
        /// </summary>
        /// <param name="daysThreshold">Ngưỡng cảnh báo (số ngày, mặc định 30)</param>
        /// <returns>Danh sách sản phẩm sắp hết hạn với ưu tiên xuất kho</returns>
        [HttpGet("risk/expiry-analysis")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetExpiryDateAnalysis([FromQuery] int daysThreshold = 30)
        {
            var result = await _dashboardService.GetExpiryDateAnalysisAsync(daysThreshold);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        /// <summary>
        /// [RỦI RO] Báo cáo Hàng quá hạn (Dead Stock) - Tính thiệt hại và truy trách nhiệm
        /// </summary>
        /// <returns>Báo cáo chi tiết hàng quá hạn với tổng thiệt hại</returns>
        [HttpGet("risk/dead-stock-report")]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> GetDeadStockReport()
        {
            var result = await _dashboardService.GetDeadStockReportAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }

        // ============================================
        // TỔNG QUAN
        // ============================================

        /// <summary>
        /// [TỔNG QUAN] Dashboard - Tất cả chỉ số chính
        /// </summary>
        /// <returns>Tổng hợp các chỉ số: Tài chính, Vận hành, Rủi ro</returns>
        [HttpGet("overview")]
        [Authorize(Policy = "StaffOrManager")]
        public async Task<IActionResult> GetDashboardOverview()
        {
            var result = await _dashboardService.GetDashboardOverviewAsync();
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            
            return Ok(result);
        }
    }
}
