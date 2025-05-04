using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.Services;

namespace SharedExperinces.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.Admin)]
public class LogsController : ControllerBase
{
    private readonly LogService _svc;
    public LogsController(LogService svc) => _svc = svc;

    [HttpGet("diagnostics")]
    public async Task<ActionResult<DiagnosticInfo>> Diagnostics()
        => Ok(await _svc.GetDiagnosticInfoAsync());

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] LogSearchQuery q)
    {
        var (logs, total) = await _svc.SearchAsync(q);
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(new
        {
            Logs = logs,
            TotalCount = total,
            q.Page,
            q.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)q.PageSize)
        });
    }

    [HttpGet("operation-types")]
    public async Task<ActionResult<IEnumerable<OperationTypeCount>>> OperationTypes()
        => Ok(await _svc.GetOperationTypesAsync());
}
