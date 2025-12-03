using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.ApiCore.Auth; // ClaimUtils
using SWS.BusinessObjects.DTOs;
using SWS.Services.ApiModels.ImportOrders;
using SWS.Services.ImportOrders;
// nếu ReviewImportOrderRequest nằm namespace khác thì thêm using tương ứng

namespace SWS.ApiCore.Controllers;

[ApiController]
[Route("api/import-orders")]
public class ImportOrdersController : ControllerBase
{
    private readonly IImportOrderQueryService _query;
    private readonly IImportOrderCommandService _cmd;

    public ImportOrdersController(IImportOrderQueryService query, IImportOrderCommandService cmd)
    {
        _query = query;
        _cmd = cmd;
    }

    /// <summary>Danh sách Import Orders (filter + paging)</summary>
    [HttpGet]
     [Authorize] // cần đăng nhập (role nào cũng được)
    public async Task<IActionResult> GetList(
        [FromQuery] string? q,
        [FromQuery] int? providerId,
        [FromQuery] string? status,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new ImportOrderListQuery(q, providerId, status, from, to, page, pageSize);
        var result = await _query.GetListAsync(query, ct);
        return Ok(result);
    }

    /// <summary>Chi tiết Import Order</summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetDetail([FromRoute] int id, CancellationToken ct = default)
    {
        var dto = await _query.GetDetailAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Tạo Import Order (Staff role=1)</summary>
    [HttpPost]
    //[Authorize(Roles = "1")]
    public async Task<IActionResult> Create([FromBody] CreateImportOrderRequest req, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var uid = ClaimUtils.GetUserIdOrThrow(User); // đã có helper bạn gửi
        var result = await _cmd.CreateAsync(uid, req, ct);
        return CreatedAtAction(nameof(GetDetail), new { id = result.ImportOrderId }, result);
    }

    /// <summary>
    /// Review Import Order:
    /// - approve = true  => Pending -> Completed (cộng tồn kho)
    /// - approve = false => Pending -> Canceled  (không cộng tồn)
    /// </summary>
    [HttpPut("{id:int}/review")]
    [Authorize(Roles = "2")] //chỉ manager được duyệt
    public async Task<IActionResult> Review(
        [FromRoute] int id,
        [FromBody] ReviewImportOrderRequest req,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var uid = ClaimUtils.GetUserIdOrThrow(User);

        var result = await _cmd.ReviewAsync(id, uid, req, ct);

        if (!result.IsSuccess)
        {
            // trả đúng status code + body từ service
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }
}
