using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;
using System.Security.Claims;

namespace SharedExperinces.WebApi.Services;

public class LogService
{
    private readonly IMongoCollection<LogEntry> _logs;
    private readonly IMongoClient _client;
    private readonly string _dbName;
    private readonly string _collectionName;
    private readonly Serilog.ILogger _logger;

    public LogService(IMongoClient client, IOptions<LoggingDatabaseSettings> opt)
    {
        _client = client;
        _dbName = opt.Value.DatabaseName;
        _collectionName = opt.Value.LogCollectionName ?? "logs";
        _logger = Log.ForContext<LogService>();

        _logs = client.GetDatabase(_dbName)
                      .GetCollection<LogEntry>(_collectionName);
    }

    /* -------------------------------------------------------------------- */
    public void LogUserActivity(ClaimsPrincipal? user, string activity, string? details = null, string? method = null)
    {
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = user?.Identity?.Name;
        var email = user?.FindFirstValue(ClaimTypes.Email);
        var roles = user?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserName", userName))
        using (LogContext.PushProperty("UserEmail", email))
        using (LogContext.PushProperty("UserRole", string.Join(", ", roles)))
        using (LogContext.PushProperty("Activity", activity))
        using (LogContext.PushProperty("Method", method ?? "USER_ACTION"))
        using (LogContext.PushProperty("Description", activity))
        {
            _logger.Information("{Activity}: {Details}", activity, details ?? string.Empty);
        }
    }

    /* -------------------------------------------------------------------- */
    public void LogError(Exception ex, string context, object? additionalData = null)
    {
        using (LogContext.PushProperty("Context", context))
        {
            if (additionalData != null)
            {
                _logger.Error(ex, "{Context} error: {Message} {@AdditionalData}", 
                    context, ex.Message, additionalData);
            }
            else
            {
                _logger.Error(ex, "{Context} error: {Message}", context, ex.Message);
            }
        }
    }

    /* -------------------------------------------------------------------- */
    public void LogOperation(string operation, string? userId = null, string? details = null, LogEventLevel level = LogEventLevel.Information)
    {
        using (LogContext.PushProperty("Operation", operation))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("Method", "API_OPERATION"))
        using (LogContext.PushProperty("Description", operation))
        {
            switch (level)
            {
                case LogEventLevel.Verbose:
                    _logger.Verbose("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
                case LogEventLevel.Debug:
                    _logger.Debug("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
                case LogEventLevel.Warning:
                    _logger.Warning("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
                case LogEventLevel.Error:
                    _logger.Error("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
                case LogEventLevel.Fatal:
                    _logger.Fatal("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
                default:
                    _logger.Information("{Operation}: {Details}", operation, details ?? string.Empty);
                    break;
            }
        }
    }

    /* -------------------------------------------------------------------- */
    public async Task<DiagnosticInfo> GetDiagnosticInfoAsync()
    {
        var info = new DiagnosticInfo
        {
            DatabaseName = _dbName,
            CollectionName = _collectionName
        };

        try
        {
            await _client.GetDatabase("admin")
                         .RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }");

            info.TotalLogCount = await _logs.CountDocumentsAsync(FilterDefinition<LogEntry>.Empty);

            if (info.TotalLogCount > 0)
            {
                var raw = await _client.GetDatabase(_dbName)
                                       .GetCollection<BsonDocument>(_collectionName)
                                       .Find(new BsonDocument())
                                       .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
                                       .Limit(1)
                                       .FirstAsync();

                info.RawDocumentStructure = raw.ToString();

                var sample = BsonSerializer.Deserialize<LogEntry>(raw);
                info.HasSampleLog = true;
                info.SampleLogId = sample.Id;
                info.SampleLogTime = sample.Timestamp;
                info.SampleLogMessage = sample.RenderedMessage;
                info.HasUserIdField = sample.Properties?.UserId != null;
                info.SampleUserId = sample.Properties?.UserId;
            }

            info.Success = true;
            _logger.Information("Diagnostic info retrieved successfully");
        }
        catch (Exception ex)
        {
            info.Success = false;
            info.ErrorMessage = ex.Message;
            _logger.Error(ex, "Failed to retrieve diagnostic info");
        }

        return info;
    }

    /* -------------------------------------------------------------------- */
    public async Task<(IEnumerable<LogEntry> Logs, long Total)> SearchAsync(LogSearchQuery q)
    {
        var f = Builders<LogEntry>.Filter;
        var filters = new List<FilterDefinition<LogEntry>>();

        if (!string.IsNullOrWhiteSpace(q.UserId))
        {
            filters.Add(f.Eq("Properties.UserId", q.UserId) |
                        f.Regex("Properties.UserName", new BsonRegularExpression(q.UserId, "i")) |
                        f.Regex("Properties.UserEmail", new BsonRegularExpression(q.UserId, "i")) |
                        f.Regex("RenderedMessage", new BsonRegularExpression(q.UserId, "i")));
        }

        if (q.StartDate.HasValue) filters.Add(f.Gte(l => l.Timestamp, q.StartDate.Value));
        if (q.EndDate.HasValue) filters.Add(f.Lte(l => l.Timestamp, q.EndDate.Value));

        if (!string.IsNullOrWhiteSpace(q.Method))
        {
            var m = q.Method.ToUpper();
            filters.Add(f.Eq("Properties.Method", m) | 
                        f.Eq("Properties.RequestMethod", m));
        }

        if (!string.IsNullOrWhiteSpace(q.Description))
        {
            filters.Add(f.Regex("Properties.Description", new BsonRegularExpression(q.Description, "i")) |
                        f.Regex("Properties.Activity", new BsonRegularExpression(q.Description, "i")) |
                        f.Regex("Properties.Operation", new BsonRegularExpression(q.Description, "i")) |
                        f.Regex("RenderedMessage", new BsonRegularExpression(q.Description, "i")));
        }

        var combined = filters.Count == 0 ? f.Empty : f.And(filters);
        var total = await _logs.CountDocumentsAsync(combined);

        var page = q.Page <= 0 ? 1 : q.Page;
        var size = q.PageSize is <= 0 or > 100 ? 20 : q.PageSize;

        var data = await _logs.Find(combined)
                              .SortByDescending(l => l.Timestamp)
                              .Skip((page - 1) * size)
                              .Limit(size)
                              .ToListAsync();

        _logger.Debug("Log search: found {Count} logs out of {Total}", data.Count, total);
        return (data, total);
    }

    /* -------------------------------------------------------------------- */
    public async Task<IReadOnlyList<OperationTypeCount>> GetOperationTypesAsync()
    {
        var mFilter = Builders<LogEntry>.Filter.In("Properties.Method",
                                                   new[] { "POST", "PUT", "DELETE" });
        var rmFilter = Builders<LogEntry>.Filter.In("Properties.RequestMethod",
                                                    new[] { "POST", "PUT", "DELETE" });

        var logs = await _logs.Find(Builders<LogEntry>.Filter.Or(mFilter, rmFilter))
                              .Limit(1_000)                  // safeguard
                              .ToListAsync();

        var result = logs.GroupBy(l => l.ActionDescription ?? "Unknown")
                   .Select(g => new OperationTypeCount
                   {
                       Description = g.Key,
                       Count = g.Count()
                   })
                   .OrderByDescending(x => x.Count)
                   .ToList();

        _logger.Debug("Retrieved {Count} operation types", result.Count);
        return result;
    }
}
