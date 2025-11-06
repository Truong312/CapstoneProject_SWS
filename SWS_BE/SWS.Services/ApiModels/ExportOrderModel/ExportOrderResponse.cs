using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;

namespace SWS.Services.ApiModels.ExportOrderModel
{
    public class ExportOrderResponse
    {
        public int ExportOrderId { get; set; }

        public string? InvoiceNumber { get; set; }

        public DateOnly OrderDate { get; set; }

        public int CustomerId { get; set; }

        public string? Currency { get; set; }

        public DateOnly? CreatedDate { get; set; }

        public DateOnly? ShippedDate { get; set; }

        public string? ShippedAddress { get; set; }

        public decimal? TaxRate { get; set; }

        public decimal? TaxAmount { get; set; }

        public decimal? TotalPayment { get; set; }

        public string? Description { get; set; }

        public StatusEnums? Status { get; set; }

        public int? CreatedBy { get; set; }
    }
}
