using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.ApiModels.ExportOrderModel
{
    public class ExportDetailResponse
    {
        public int ExportDetailId { get; set; }

        public int ExportOrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
