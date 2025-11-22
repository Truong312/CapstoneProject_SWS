using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.Services.InventoryServices
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InventoryStatusSummaryDto> GetInventoryStatusSummaryAsync()
        {
            return await _unitOfWork.Inventory.GetStockByStatusAsync();
        }
        public async Task<List<ProductInventoryDto>> GetAllProductInventoryAsync()
        {
            return await _unitOfWork.Inventory.GetAllProductInventoryAsync();
        }


    }
}
