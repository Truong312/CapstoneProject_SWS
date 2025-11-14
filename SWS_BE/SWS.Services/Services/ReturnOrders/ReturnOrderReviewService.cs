// File: SWS.Services/ReturnOrders/ReturnOrderReviewService.cs
using SWS.BusinessObjects.Constants;
using SWS.BusinessObjects.DTOs;
using SWS.BusinessObjects.Models;
using SWS.Repositories.UnitOfWork;

namespace SWS.Services.ReturnOrders
{
    public interface IReturnOrderReviewService
    {
        Task<ReviewReturnOrderResultDto> ReviewAsync(int managerUserId, ReviewReturnOrderRequest req, CancellationToken ct = default);
    }

    public class ReturnOrderReviewService : IReturnOrderReviewService
    {
        private readonly IUnitOfWork _uow;
        public ReturnOrderReviewService(IUnitOfWork uow) => _uow = uow;

        public async Task<ReviewReturnOrderResultDto> ReviewAsync(int managerUserId, ReviewReturnOrderRequest req, CancellationToken ct = default)
        {
            var ro = await _uow.ReturnOrdersCommand.GetForUpdateAsync(req.ReturnOrderId, ct);
            if (ro == null) throw new KeyNotFoundException("Return order not found.");

            // Nếu status null => coi như Pending
            ro.Status ??= ReturnStatuses.Pending;

            if (!ro.Status.Equals(ReturnStatuses.Pending, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Current status '{ro.Status}' cannot be reviewed.");

            // Gán người review + trạng thái
            ro.ReviewedBy = managerUserId;

            var approve = (req.Decision ?? "approve").Equals("approve", StringComparison.OrdinalIgnoreCase);
            ro.Status = approve ? ReturnStatuses.Approved : ReturnStatuses.Rejected;

            // Ghi chú
            if (!string.IsNullOrWhiteSpace(req.Note))
                ro.Note = string.IsNullOrWhiteSpace(ro.Note)
                    ? req.Note
                    : $"{ro.Note}\n[Review] {req.Note}";

            // Cập nhật từng dòng (nếu có gửi)
            if (req.Lines != null && req.Lines.Count > 0)
            {
                var linesMap = ro.ReturnOrderDetails.ToDictionary(x => x.ReturnDetailId);
                foreach (var l in req.Lines)
                {
                    if (linesMap.TryGetValue(l.ReturnDetailId, out var entity))
                    {
                        if (l.ActionId.HasValue) entity.ActionId = l.ActionId;
                        if (!string.IsNullOrWhiteSpace(l.Note)) entity.Note = l.Note;
                    }
                }
            }

            await _uow.SaveChangesAsync();

            // Ghi ActionLog
            await _uow.ReturnOrdersCommand.AddActionLogAsync(new ActionLog
            {
                UserId = managerUserId,
                ActionType = "REVIEW_RETURN",
                EntityType = "ReturnOrder",
                Description = $"ReturnOrder #{ro.ReturnOrderId} -> {ro.Status}",
                Timestamp = DateTime.UtcNow
            }, ct);
            await _uow.SaveChangesAsync();

            return new ReviewReturnOrderResultDto
            {
                ReturnOrderId = ro.ReturnOrderId,
                Status = ro.Status ?? string.Empty,
                ReviewedByName = ro.ReviewedByNavigation?.FullName,
                CheckInTime = ro.CheckInTime,
                Note = ro.Note
            };
        }
    }
}
