namespace SWS.Services.ApiModels.ImportOrders
{
    public class ReviewImportOrderRequest
    {
        /// <summary>
        /// true = duyệt (Completed), false = hủy (Canceled)
        /// </summary>
        public bool Approve { get; set; }

        /// <summary>
        /// Ghi chú review (lý do hủy, comment...)
        /// </summary>
        public string? Note { get; set; }
    }
}
