using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.ApiModels.LogModel
{
    public class TransactionLogResponse
    {
        public int TransactionId { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public int? CreatedBy { get; set; }

        public string? Type { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string? Notes { get; set; }

        public int? QuantityChanged { get; set; }   
    }
}
