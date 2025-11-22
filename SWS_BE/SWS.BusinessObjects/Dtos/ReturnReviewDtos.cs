using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.BusinessObjects.DTOs
{
    // Body dùng khi gọi POST /api/returns/{id}/review
    public class ReviewReturnOrderRequest
    {
        public int ReturnOrderId { get; set; }   // sẽ gán từ route id
        public string Decision { get; set; } = "approve"; // "approve" | "reject"
        public string? Note { get; set; }
        public List<ReviewReturnOrderLineDto>? Lines { get; set; }
    }

    public class ReviewReturnOrderLineDto
    {
        public int ReturnDetailId { get; set; }
        public int? ActionId { get; set; }   // tuỳ manager đổi action từng dòng
        public string? Note { get; set; }
    }

    public class ReviewReturnOrderResultDto
    {
        public int ReturnOrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ReviewedByName { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string? Note { get; set; }
    }
}
