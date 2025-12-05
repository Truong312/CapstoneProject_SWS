using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.DashboardModel;

namespace SWS.Services.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ============================================
        // 1. TÀI CHÍNH & HIỆU QUẢ
        // ============================================

        public async Task<ResultModel<List<TrendAnalysisResponse>>> GetTrendAnalysisAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var exportOrders = await _unitOfWork.ExportOrders.GetAllAsync();
                var exportDetails = await _unitOfWork.ExportDetails.GetAllAsync();
                var importOrders = await _unitOfWork.ImportOrdersQuery.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var products = await _unitOfWork.Products.GetAllAsync();

                var result = new List<TrendAnalysisResponse>();

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    // Đơn xuất trong ngày
                    var dayExports = exportOrders.Where(o => o.OrderDate == date).ToList();
                    var dayExportDetails = exportDetails.Where(ed => 
                        dayExports.Any(e => e.ExportOrderId == ed.ExportOrderId)).ToList();

                    // Đơn nhập trong ngày
                    var dayImports = importOrders.Where(o => o.OrderDate == date).ToList();

                    // Tính giá trị và số lượng
                    var exportValue = dayExportDetails.Sum(ed => ed.TotalPrice ?? 0);
                    var exportQty = dayExportDetails.Sum(ed => ed.Quantity);
                    
                    // Giá trị tồn kho ước tính
                    var stockValue = inventories.Sum(i => {
                        var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                        return i.QuantityAvailable * (product?.UnitPrice ?? 0);
                    });
                    var stockQty = inventories.Sum(i => i.QuantityAvailable);

                    result.Add(new TrendAnalysisResponse
                    {
                        Date = date,
                        ImportValue = 0, // Cần tính từ ImportDetails
                        ExportValue = exportValue,
                        StockValue = stockValue,
                        ImportQuantity = 0,
                        ExportQuantity = exportQty,
                        StockQuantity = stockQty
                    });
                }

                return new ResultModel<List<TrendAnalysisResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Phân tích xu hướng thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<TrendAnalysisResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<PeriodComparisonResponse>>> GetPeriodComparisonAsync(string periodType, int count = 6)
        {
            try
            {
                var exportOrders = await _unitOfWork.ExportOrders.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var products = await _unitOfWork.Products.GetAllAsync();

                var result = new List<PeriodComparisonResponse>();
                var today = DateOnly.FromDateTime(DateTime.Now);

                for (int i = 0; i < count; i++)
                {
                    DateOnly periodStart, periodEnd;
                    string periodName;

                    if (periodType.ToLower() == "month")
                    {
                        var targetDate = today.AddMonths(-i);
                        periodStart = new DateOnly(targetDate.Year, targetDate.Month, 1);
                        periodEnd = periodStart.AddMonths(1).AddDays(-1);
                        periodName = $"Tháng {targetDate.Month}/{targetDate.Year}";
                    }
                    else // quarter
                    {
                        var quarter = (today.Month - 1) / 3 + 1 - i;
                        var year = today.Year;
                        if (quarter <= 0)
                        {
                            quarter += 4;
                            year--;
                        }
                        periodStart = new DateOnly(year, (quarter - 1) * 3 + 1, 1);
                        periodEnd = periodStart.AddMonths(3).AddDays(-1);
                        periodName = $"Q{quarter}/{year}";
                    }

                    // Số dư đầu kỳ (giả định từ tồn kho hiện tại)
                    var openingStockValue = inventories.Sum(inv => {
                        var product = products.FirstOrDefault(p => p.ProductId == inv.ProductId);
                        return inv.QuantityAvailable * (product?.PurchasedPrice ?? 0);
                    });
                    var openingStockQty = inventories.Sum(i => i.QuantityAvailable);

                    // Doanh thu trong kỳ
                    var periodOrders = exportOrders.Where(o => 
                        o.OrderDate >= periodStart && o.OrderDate <= periodEnd).ToList();
                    var revenue = periodOrders.Sum(o => o.TotalPayment ?? 0);

                    // Số dư cuối kỳ (trong thực tế cần lưu snapshot)
                    var closingStockValue = openingStockValue; // Tạm thời
                    var closingStockQty = openingStockQty;

                    // Tính tăng trưởng
                    var valueGrowth = openingStockValue > 0 
                        ? ((closingStockValue - openingStockValue) / openingStockValue) * 100 
                        : 0;
                    var qtyGrowth = openingStockQty > 0 
                        ? ((decimal)(closingStockQty - openingStockQty) / openingStockQty) * 100 
                        : 0;

                    // Cảnh báo ứ đọng vốn
                    var isStagnation = valueGrowth > 5 && revenue == 0;

                    result.Add(new PeriodComparisonResponse
                    {
                        PeriodName = periodName,
                        StartDate = periodStart,
                        EndDate = periodEnd,
                        OpeningStockValue = openingStockValue,
                        OpeningStockQuantity = openingStockQty,
                        ClosingStockValue = closingStockValue,
                        ClosingStockQuantity = closingStockQty,
                        ValueGrowthPercent = valueGrowth,
                        QuantityGrowthPercent = qtyGrowth,
                        Revenue = revenue,
                        IsCapitalStagnation = isStagnation,
                        Warning = isStagnation ? "⚠️ Tồn kho tăng nhưng doanh thu không tăng - Nguy cơ ứ đọng vốn!" : ""
                    });
                }

                result.Reverse(); // Sắp xếp từ cũ đến mới

                return new ResultModel<List<PeriodComparisonResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "So sánh tăng trưởng theo kỳ thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<PeriodComparisonResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<ValueVsQuantityResponse>>> GetValueVsQuantityAnalysisAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var locations = await _unitOfWork.Locations.GetAllAsync();

                var result = products.Select(p =>
                {
                    var productInventories = inventories.Where(i => i.ProductId == p.ProductId).ToList();
                    var totalQty = productInventories.Sum(i => i.QuantityAvailable);
                    var locationInfos = productInventories
                        .Select(inv => locations.FirstOrDefault(l => l.LocationId == inv.LocationId)?.ShelfId ?? "N/A")
                        .Distinct();

                    return new ValueVsQuantityResponse
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        SerialNumber = p.SerialNumber,
                        
                        // Cho Kế toán
                        UnitCost = p.PurchasedPrice ?? 0,
                        TotalValue = totalQty * (p.PurchasedPrice ?? 0),
                        AverageCost = p.PurchasedPrice ?? 0,
                        
                        // Cho Thủ kho
                        TotalQuantity = totalQty,
                        LocationInfo = string.Join(", ", locationInfos),
                        OccupiedSpace = totalQty * 0.1m // Giả định mỗi sản phẩm chiếm 0.1m²
                    };
                }).ToList();

                return new ResultModel<List<ValueVsQuantityResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Phân tích Giá trị vs Số lượng thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<ValueVsQuantityResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        // ============================================
        // 2. VẬN HÀNH & TỐI ƯU
        // ============================================

        public async Task<ResultModel<List<Top10StructureAnalysisResponse>>> GetTop10StructureAnalysisAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();

                // Tính tổng giá trị kho
                var totalWarehouseValue = inventories.Sum(i => {
                    var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                    return i.QuantityAvailable * (product?.UnitPrice ?? 0);
                });

                // Phân tích từng sản phẩm
                var productAnalysis = products.Select(p =>
                {
                    var productInventories = inventories.Where(i => i.ProductId == p.ProductId).ToList();
                    var totalQty = productInventories.Sum(i => i.QuantityAvailable);
                    var totalValue = totalQty * (p.UnitPrice ?? 0);
                    var valuePercent = totalWarehouseValue > 0 ? (totalValue / totalWarehouseValue) * 100 : 0;

                    return new
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        SerialNumber = p.SerialNumber,
                        TotalValue = totalValue,
                        TotalQuantity = totalQty,
                        ValuePercentage = valuePercent
                    };
                })
                .OrderByDescending(x => x.TotalValue)
                .ToList();

                // Tính % tích lũy và phân loại ABC
                decimal cumulativePercent = 0;
                var result = productAnalysis.Take(10).Select((item, index) =>
                {
                    cumulativePercent += item.ValuePercentage;
                    
                    string category;
                    string priority;
                    
                    if (cumulativePercent <= 80)
                    {
                        category = "A";
                        priority = "🔴 CAO - Kiểm soát chặt chẽ, kiểm kê thường xuyên";
                    }
                    else if (cumulativePercent <= 95)
                    {
                        category = "B";
                        priority = "🟡 TRUNG BÌNH - Kiểm soát định kỳ";
                    }
                    else
                    {
                        category = "C";
                        priority = "🟢 THẤP - Kiểm soát cơ bản";
                    }

                    return new Top10StructureAnalysisResponse
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        SerialNumber = item.SerialNumber,
                        TotalValue = item.TotalValue,
                        TotalQuantity = item.TotalQuantity,
                        ValuePercentage = Math.Round(item.ValuePercentage, 2),
                        CumulativePercentage = Math.Round(cumulativePercent, 2),
                        Category = category,
                        ManagementPriority = priority
                    };
                }).ToList();

                return new ResultModel<List<Top10StructureAnalysisResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Phân tích cấu trúc Top 10 (Quy luật 80/20) thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<Top10StructureAnalysisResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<WarehouseBalanceResponse>>> GetWarehouseBalanceAnalysisAsync()
        {
            try
            {
                var locations = await _unitOfWork.Locations.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var products = await _unitOfWork.Products.GetAllAsync();

                var result = locations.Select(loc =>
                {
                    var locInventories = inventories.Where(i => i.LocationId == loc.LocationId).ToList();
                    var totalProducts = locInventories.Select(i => i.ProductId).Distinct().Count();
                    var totalQty = locInventories.Sum(i => i.QuantityAvailable);
                    var totalValue = locInventories.Sum(i => {
                        var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                        return i.QuantityAvailable * (product?.UnitPrice ?? 0);
                    });

                    // Giả định công suất tối đa là 1000 sản phẩm
                    var maxCapacity = 1000m;
                    var capacityUsed = (totalQty / maxCapacity) * 100;
                    var isOverloaded = capacityUsed > 90;
                    var isUnderUtilized = capacityUsed < 30;

                    return new WarehouseBalanceResponse
                    {
                        LocationId = loc.LocationId,
                        LocationName = $"Kho {loc.ShelfId}",
                        ShelfId = loc.ShelfId ?? "",
                        TotalProducts = totalProducts,
                        TotalQuantity = totalQty,
                        TotalValue = totalValue,
                        CapacityUsed = Math.Round(capacityUsed, 2),
                        IsOverloaded = isOverloaded,
                        IsUnderUtilized = isUnderUtilized,
                        TransferSuggestions = new List<TransferSuggestion>()
                    };
                }).ToList();

                return new ResultModel<List<WarehouseBalanceResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Phân tích cân bằng kho thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<WarehouseBalanceResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        // ============================================
        // 3. QUẢN TRỊ RỦI RO
        // ============================================

        public async Task<ResultModel<List<MinimumStockAlertResponse>>> GetMinimumStockAlertsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var businessPartners = await _unitOfWork.Accounts.GetAllAsync();

                var alerts = products.Select(p =>
                {
                    var currentStock = inventories.Where(i => i.ProductId == p.ProductId)
                        .Sum(i => i.QuantityAvailable);
                    var reorderPoint = p.ReorderPoint ?? 10;
                    var minimumStock = reorderPoint / 2;

                    if (currentStock >= reorderPoint)
                        return null;

                    var shortage = reorderPoint - currentStock;
                    var suggestedOrderQty = Math.Max(shortage * 2, reorderPoint); // Đặt gấp đôi để an toàn

                    string alertLevel;
                    if (currentStock <= minimumStock)
                        alertLevel = "🔴 CRITICAL";
                    else if (currentStock <= reorderPoint * 0.75)
                        alertLevel = "🟡 WARNING";
                    else
                        alertLevel = "🟢 INFO";

                    return new MinimumStockAlertResponse
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        SerialNumber = p.SerialNumber,
                        CurrentStock = currentStock,
                        MinimumStock = minimumStock,
                        ReorderPoint = reorderPoint,
                        ShortageQuantity = shortage,
                        AlertLevel = alertLevel,
                        SuggestedOrderQuantity = suggestedOrderQty,
                        EstimatedCost = suggestedOrderQty * (p.PurchasedPrice ?? 0),
                        LeadTimeDays = 7, // Giả định 7 ngày
                        SuggestedOrderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                        SupplierId = 0,
                        SupplierName = "Cần xác định"
                    };
                })
                .Where(a => a != null)
                .Select(a => a!)
                .OrderBy(a => a.CurrentStock)
                .ToList();

                return new ResultModel<List<MinimumStockAlertResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"Tìm thấy {alerts.Count} cảnh báo tồn kho tối thiểu",
                    Data = alerts
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<MinimumStockAlertResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<ExpiryDateAnalysisResponse>>> GetExpiryDateAnalysisAsync(int daysThreshold = 30)
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var locations = await _unitOfWork.Locations.GetAllAsync();

                var today = DateOnly.FromDateTime(DateTime.Now);
                var thresholdDate = today.AddDays(daysThreshold);

                var result = products
                    .Where(p => p.ExpiredDate <= thresholdDate)
                    .Select(p =>
                    {
                        var productInventories = inventories.Where(i => i.ProductId == p.ProductId).ToList();
                        var totalQty = productInventories.Sum(i => i.QuantityAvailable);
                        var locationInfos = productInventories
                            .Select(inv => locations.FirstOrDefault(l => l.LocationId == inv.LocationId)?.ShelfId ?? "N/A")
                            .Distinct();

                        var daysUntilExpiry = p.ExpiredDate.DayNumber - today.DayNumber;
                        
                        string expiryStatus;
                        string fefoPriority;
                        var suggestions = new List<string>();

                        if (daysUntilExpiry < 0)
                        {
                            expiryStatus = "⚫ EXPIRED";
                            fefoPriority = "URGENT";
                            suggestions.Add("Ngừng xuất ngay, chuyển sang xử lý hủy");
                        }
                        else if (daysUntilExpiry <= 7)
                        {
                            expiryStatus = "🔴 CRITICAL";
                            fefoPriority = "HIGH";
                            suggestions.Add("Ưu tiên xuất kho ngay trong 7 ngày");
                            suggestions.Add("Cân nhắc khuyến mãi/giảm giá");
                        }
                        else if (daysUntilExpiry <= 30)
                        {
                            expiryStatus = "🟡 NEAR_EXPIRY";
                            fefoPriority = "MEDIUM";
                            suggestions.Add("Sắp xếp ưu tiên xuất trước hàng khác");
                        }
                        else
                        {
                            expiryStatus = "🟢 SAFE";
                            fefoPriority = "LOW";
                            suggestions.Add("Kiểm tra định kỳ");
                        }

                        return new ExpiryDateAnalysisResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.Name,
                            SerialNumber = p.SerialNumber,
                            BatchNumber = "N/A", // Cần thêm field BatchNumber vào Product
                            ExpiryDate = p.ExpiredDate,
                            DaysUntilExpiry = daysUntilExpiry,
                            Quantity = totalQty,
                            Value = totalQty * (p.UnitPrice ?? 0),
                            LocationInfo = string.Join(", ", locationInfos),
                            ExpiryStatus = expiryStatus,
                            FEFOPriority = fefoPriority,
                            ActionSuggestions = suggestions
                        };
                    })
                    .OrderBy(x => x.DaysUntilExpiry)
                    .ToList();

                return new ResultModel<List<ExpiryDateAnalysisResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"Phân tích {result.Count} sản phẩm theo hạn sử dụng (FEFO)",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<ExpiryDateAnalysisResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<DeadStockReportResponse>>> GetDeadStockReportAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var locations = await _unitOfWork.Locations.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();

                var today = DateOnly.FromDateTime(DateTime.Now);

                var deadStock = products
                    .Where(p => p.ExpiredDate < today)
                    .Select(p =>
                    {
                        var productInventories = inventories.Where(i => i.ProductId == p.ProductId).ToList();
                        var totalQty = productInventories.Sum(i => i.QuantityAvailable);
                        var locationInfos = productInventories
                            .Select(inv => locations.FirstOrDefault(l => l.LocationId == inv.LocationId)?.ShelfId ?? "N/A")
                            .Distinct();

                        var daysOverdue = today.DayNumber - p.ExpiredDate.DayNumber;
                        var originalValue = totalQty * (p.PurchasedPrice ?? 0);
                        var liquidationValue = originalValue * 0.1m; // Giả định thanh lý được 10%
                        var totalLoss = originalValue - liquidationValue;

                        return new DeadStockReportResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.Name,
                            SerialNumber = p.SerialNumber,
                            BatchNumber = "N/A",
                            ExpiryDate = p.ExpiredDate,
                            DaysOverdue = daysOverdue,
                            Quantity = totalQty,
                            OriginalValue = originalValue,
                            LiquidationValue = liquidationValue,
                            TotalLoss = totalLoss,
                            LocationInfo = string.Join(", ", locationInfos),
                            ResponsibleUserId = null,
                            ResponsibleUserName = "Cần điều tra",
                            ResponsibleRole = "N/A",
                            ImportDate = p.ReceivedDate,
                            ImportOrderId = 0,
                            DisposalStatus = "Pending",
                            DisposalMethod = "Cần quyết định"
                        };
                    })
                    .OrderByDescending(x => x.TotalLoss)
                    .ToList();

                var totalLoss = deadStock.Sum(d => d.TotalLoss);

                return new ResultModel<List<DeadStockReportResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"⚠️ Tìm thấy {deadStock.Count} sản phẩm quá hạn. Tổng thiệt hại: {totalLoss:N0} VNĐ",
                    Data = deadStock
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<DeadStockReportResponse>>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        // ============================================
        // TỔNG QUAN
        // ============================================

        public async Task<ResultModel<DashboardOverviewResponse>> GetDashboardOverviewAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var locations = await _unitOfWork.Locations.GetAllAsync();
                var exportOrders = await _unitOfWork.ExportOrders.GetAllAsync();

                var today = DateOnly.FromDateTime(DateTime.Now);
                var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

                // Tài chính
                var totalStockValue = inventories.Sum(i => {
                    var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                    return i.QuantityAvailable * (product?.UnitPrice ?? 0);
                });
                var monthlyRevenue = exportOrders
                    .Where(o => o.OrderDate >= firstDayOfMonth)
                    .Sum(o => o.TotalPayment ?? 0);

                // Vận hành
                var totalProducts = products.Count();
                var totalLocations = locations.Count();

                // Rủi ro
                var lowStockCount = products.Count(p => {
                    var stock = inventories.Where(i => i.ProductId == p.ProductId).Sum(i => i.QuantityAvailable);
                    return stock < (p.ReorderPoint ?? 10);
                });
                var nearExpiryCount = products.Count(p => p.ExpiredDate <= today.AddDays(30) && p.ExpiredDate >= today);
                var expiredCount = products.Count(p => p.ExpiredDate < today);
                var potentialLoss = products
                    .Where(p => p.ExpiredDate < today)
                    .Sum(p => {
                        var qty = inventories.Where(i => i.ProductId == p.ProductId).Sum(i => i.QuantityAvailable);
                        return qty * (p.PurchasedPrice ?? 0);
                    });

                var overview = new DashboardOverviewResponse
                {
                    TotalStockValue = totalStockValue,
                    MonthlyRevenue = monthlyRevenue,
                    ValueGrowthRate = 0, // Cần tính so với tháng trước
                    TotalProducts = totalProducts,
                    TotalLocations = totalLocations,
                    AverageCapacityUsage = 65, // Giả định
                    LowStockAlertCount = lowStockCount,
                    NearExpiryCount = nearExpiryCount,
                    ExpiredStockCount = expiredCount,
                    TotalPotentialLoss = potentialLoss
                };

                return new ResultModel<DashboardOverviewResponse>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Dashboard tổng quan",
                    Data = overview
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<DashboardOverviewResponse>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }
    }
}

