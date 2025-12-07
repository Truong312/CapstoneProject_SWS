using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.InventoryModel;
using System.Security.Claims;
using SWS.BusinessObjects.Enums;
using SWS.Services.Services.LogServices;

namespace SWS.Services.Services.InventoryServices
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActionLogService _actionLogService;
        public InventoryService(IUnitOfWork unitOfWork, IActionLogService actionLogService)
        {
            _unitOfWork = unitOfWork;
            _actionLogService = actionLogService;
        }
        public async Task<ResultModel<IEnumerable<InventoryResponse>>> GetAllInventoriesAsync()
        {
            try
            {
                var inventories = await _unitOfWork.Inventories.GetAllAsync();
                var result = inventories.Select(i => new InventoryResponse
                {
                    InventoryId = i.InventoryId,
                    ProductId = i.ProductId,
                    QuantityAvailable = i.QuantityAvailable,
                    AllocatedQuantity = i.AllocatedQuantity,
                    LocationId = i.LocationId
                });
                return new ResultModel<IEnumerable<InventoryResponse>>
                {
                    IsSuccess = true,
                    Message = "Danh sách số lượng sản phẩm trong kho hàng",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<IEnumerable<InventoryResponse>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách số lượng sản phẩm trong kho hàng: {e.Message}",
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }
        public async Task<ResultModel<InventoryResponse>> GetInventoryByIdAsync(int inventoryId)
        {
            try
            {
                var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
                if (inventory == null)
                {
                    return new ResultModel<InventoryResponse>
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy kho hàng trong kho với id={inventoryId}",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                var result = new InventoryResponse
                {
                    InventoryId = inventory.InventoryId,
                    ProductId = inventory.ProductId,
                    QuantityAvailable = inventory.QuantityAvailable,
                    AllocatedQuantity = inventory.AllocatedQuantity,
                    LocationId = inventory.LocationId
                };
                return new ResultModel<InventoryResponse>
                {
                    IsSuccess = true,
                    Message = $"Kho hàng với id={inventoryId}",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<InventoryResponse>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm sản phẩm trong kho: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel<InventoryResponse>> GetInventoryByProductIdAsync(int productId)
        {
            try
            {
                var inventory = await _unitOfWork.Inventories.GetSingleAsync(i => i.ProductId == productId);
                if (inventory == null)
                {
                    return new ResultModel<InventoryResponse>
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy mặt hàng trong kho với id={productId}",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                var result = new InventoryResponse
                {
                    InventoryId = inventory.InventoryId,
                    ProductId = inventory.ProductId,
                    QuantityAvailable = inventory.QuantityAvailable,
                    AllocatedQuantity = inventory.AllocatedQuantity,
                    LocationId = inventory.LocationId
                };
                return new ResultModel<InventoryResponse>
                {
                    IsSuccess = true,
                    Message = $"Kho hàng trong kho với id mặt hàng={productId}",
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                return new ResultModel<InventoryResponse>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm sản phẩm trong kho: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> AddInventoryAsync(AddInventory addInventory)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(addInventory.ProductId);
                if (product == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không thể thêm sản phẩm vào kho do không tìm thấy mặt hàng với id={addInventory.ProductId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var inventory = new Inventory
                {
                    ProductId = addInventory.ProductId,
                    QuantityAvailable = addInventory.QuantityAvailable,
                    AllocatedQuantity = addInventory.AllocatedQuantity,
                    LocationId = addInventory.LocationId
                };
                await _unitOfWork.Inventories.AddAsync(inventory);
                await _actionLogService.CreateActionLogAsync(ActionType.Create, "Inventory", "Thêm sản phẩm vào kho");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Thêm mặt hàng vào kho thành công",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi thêm mặt hàng vào kho: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };

            }
            ;
        }
        public async Task<ResultModel> UpdateInventoryAsync(int inventoryId, UpdateInventory updateInventory)
        {
            try
            {
                var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
                if (inventory == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy kho hàng với id = {inventoryId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var location = await _unitOfWork.Locations.GetByIdAsync(updateInventory.LocationId);
                if (inventory == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy vị trí kho hàng với id = {updateInventory.LocationId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if (inventory.ProductId != updateInventory.ProductId) inventory.ProductId = updateInventory.ProductId;
                if (inventory.QuantityAvailable != updateInventory.QuantityAvailable) inventory.QuantityAvailable = updateInventory.QuantityAvailable;
                if (inventory.AllocatedQuantity != updateInventory.AllocatedQuantity) inventory.AllocatedQuantity = updateInventory.AllocatedQuantity;
                if (inventory.LocationId != updateInventory.LocationId) inventory.LocationId = updateInventory.LocationId;

                await _actionLogService.CreateActionLogAsync(ActionType.Update, "Inventory", "Cập nhật sản phẩm trong kho");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = "Đã cập nhật kho hàng thành công",
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi cập nhật kho hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> DeleteInventoryAsync(int inventoryId)
        {
            try
            {
                var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
                if (inventory == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy kho hàng với id = {inventoryId}",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                await _unitOfWork.Inventories.DeleteByIdAsync(inventoryId);
                await _actionLogService.CreateActionLogAsync(ActionType.Delete, "Inventory", "Thêm sản phẩm vào kho");
                await _unitOfWork.SaveChangesAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = $"Đã xoá kho hàng với id = {inventoryId} thành công",
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi xảy ra khi xoá kho hàng: {e.Message}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<InventoryStatusSummaryDto> GetInventoryStatusSummaryAsync()
        {
            return await _unitOfWork.Inventories.GetStockByStatusAsync();
        }
        public async Task<List<ProductInventoryDto>> GetAllProductInventoryAsync()
        {
            return await _unitOfWork.Inventories.GetAllProductInventoryAsync();
        }
    }
}
