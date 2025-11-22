using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.ApiCore.Auth; // ClaimUtils
using SWS.BusinessObjects.DTOs;
using SWS.Services.ImportOrders;

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
   /* [Authorize]*/ // cần đăng nhập (role nào cũng được)
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
}
