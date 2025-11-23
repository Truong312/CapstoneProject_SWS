//using SWS.BusinessObjects.Dtos;
//using SWS.Repositories.Repositories.InventoryRepo;
//using SWS.Repositories.UnitOfWork;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SWS.Services.Services.InventoryServices
//{
//    public class InventoryDashboardService : IInventoryDashboardService
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public InventoryDashboardService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<InventoryDashboardDto> GetDashboardAsync()
//        {
//            return new InventoryDashboardDto
//            {
//                TotalStockValue = await _unitOfWork.InventoryDashboard.GetTotalStockValueAsync(),
//                LowStockCount = await _unitOfWork.InventoryDashboard.GetLowStockCountAsync(),
//                OutOfStockCount = await _unitOfWork.InventoryDashboard.GetOutOfStockCountAsync(),
//                InventoryTurnoverRate = await _unitOfWork.InventoryDashboard.GetInventoryTurnoverRateAsync()
//            };
//        }
//    }

//}
