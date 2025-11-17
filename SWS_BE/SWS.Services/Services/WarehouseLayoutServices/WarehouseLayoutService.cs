using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Dtos.WarehouseLayout;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels;
using SWS.BusinessObjects.Dtos.WarehouseLayout;
using System;
using System.Linq;
using System.Threading.Tasks;
using SWS.Services.ApiModels;

namespace SWS.Services.Services.WarehouseLayoutServices
{
    public class WarehouseLayoutService : IWarehouseLayoutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseLayoutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultModel<WarehouseLayoutResponse>> GetLayoutAsync(string? shelfId, int? productId)
        {
            try
            {
                var locations = await _unitOfWork.Locations.GetLayoutAsync(shelfId, productId);

                var shelves = locations
                    .GroupBy(l => l.ShelfId)
                    .Select(group =>
                    {
                        var cells = group.Select(l => new WarehouseCellDto
                        {
                            LocationId = l.LocationId,
                            ShelfId = l.ShelfId,
                            RowNumber = l.RowNumber,
                            ColumnNumber = l.ColumnNumber,
                            Type = l.Type,
                            IsFull = l.IsFull ?? false,
                            Products = l.Inventories
                                .Select(inv => new WarehouseCellProductDto
                                {
                                    ProductId = inv.ProductId,
                                    ProductName = inv.Product.Name,
                                    QuantityAvailable = inv.QuantityAvailable
                                })
                                .ToList()
                        }).ToList();

                        var maxRow = group.Max(l => l.RowNumber ?? 0);
                        var maxColumn = group.Max(l => l.ColumnNumber ?? 0);

                        return new WarehouseShelfDto
                        {
                            ShelfId = group.Key,
                            MaxRow = maxRow,
                            MaxColumn = maxColumn,
                            Cells = cells
                        };
                    })
                    .OrderBy(s => s.ShelfId)
                    .ToList();

                var response = new WarehouseLayoutResponse
                {
                    Shelves = shelves
                };

                return new ResultModel<WarehouseLayoutResponse>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = response,
                    Message = "Lấy layout kho thành công."
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<WarehouseLayoutResponse>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Lỗi khi lấy layout kho: {ex.Message}"
                };
            }
        }
    }
}
