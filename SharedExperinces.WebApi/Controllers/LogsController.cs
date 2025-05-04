using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.Services;
using System.Security.Claims;

namespace SharedExperinces.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.Admin)]
public class LogsController : ControllerBase
{
    private readonly LogService _svc;
    private readonly Serilog.ILogger _logger;
    
    public LogsController(LogService svc)
    {
        _svc = svc;
        _logger = Log.ForContext<LogsController>();
    }

    [HttpGet("diagnostics")]
    public async Task<ActionResult<DiagnosticInfo>> Diagnostics()
    {
        _logger.Information("Admin accessed log diagnostics");
        return Ok(await _svc.GetDiagnosticInfoAsync());
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] LogSearchQuery q)
    {
        _svc.LogUserActivity(User, "Log search", $"Criteria: {q.Method}, {q.UserId}, {q.Description}", "GET");
        
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
    {
        _svc.LogUserActivity(User, "Retrieved operation types", null, "GET");
        return Ok(await _svc.GetOperationTypesAsync());
    }
    
    // Test endpoints for verifying POST, PUT, DELETE logging
    
    [HttpPost("test")]
    public IActionResult TestPost([FromBody] TestLogRequest request)
    {
        // Get the user ID from claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name;
        
        // Use LogContext to properly log the operation
        using (LogContext.PushProperty("RequestMethod", "POST"))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserName", userName))
        using (LogContext.PushProperty("Description", "Test POST request"))
        {
            _logger.Information("POST request received: {Message}", request.Message);
        }
        
        // Also use the LogService
        _svc.LogUserActivity(User, "Test POST request", request.Message, "POST");
        
        // Log one more direct entry to ensure MongoDB is getting something
        Log.ForContext("RequestMethod", "POST")
           .ForContext("UserDetails", new { UserId = userId, UserName = userName })
           .ForContext("Operation", "Test")
           .Information("Direct POST log entry: {Message}", request.Message);
        
        return Ok(new { Message = $"POST logged: {request.Message}", UserId = userId });
    }
    
    [HttpPut("test/{id}")]
    public IActionResult TestPut(int id, [FromBody] TestLogRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Use LogContext for proper structured logging
        using (LogContext.PushProperty("RequestMethod", "PUT"))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("Description", $"Test PUT request for ID {id}"))
        {
            _logger.Information("PUT request for ID {Id} received: {Message}", id, request.Message);
        }
        
        _svc.LogUserActivity(User, $"Test PUT request for ID {id}", request.Message, "PUT");
        return Ok(new { Message = $"PUT logged for ID {id}: {request.Message}", UserId = userId });
    }
    
    [HttpDelete("test/{id}")]
    public IActionResult TestDelete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Use LogContext for proper structured logging
        using (LogContext.PushProperty("RequestMethod", "DELETE"))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("Description", $"Test DELETE request for ID {id}"))
        {
            _logger.Information("DELETE request for ID {Id} received", id);
        }
        
        _svc.LogUserActivity(User, $"Test DELETE request for ID {id}", null, "DELETE");
        return Ok(new { Message = $"DELETE logged for ID {id}", UserId = userId });
    }
    
    // Force a log entry to MongoDB for testing
    [HttpGet("test-mongodb")]
    public IActionResult TestMongoDBLogging()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name;
        
        // Direct MongoDB log
        Log.ForContext("RequestMethod", "TEST")
           .ForContext("UserId", userId)
           .ForContext("UserName", userName)
           .ForContext("Description", "MongoDB Test Log")
           .Information("Direct MongoDB test log");
        
        // Also try via service
        _svc.LogOperation("TEST_MONGODB", userId, "Testing MongoDB logging directly");
        
        return Ok(new { Message = "MongoDB test log created", Timestamp = DateTime.UtcNow });
    }
}

public class TestLogRequest
{
    public string Message { get; set; } = string.Empty;
}
