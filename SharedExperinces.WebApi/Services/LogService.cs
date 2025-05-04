using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Services;

public class LogService
{
    private readonly IMongoCollection<LogEntry> _logs;
    private readonly IMongoClient _client;
    private readonly string _dbName;
    private readonly string _collectionName;

    public LogService(IMongoClient client, IOptions<LoggingDatabaseSettings> opt)
    {
        _client = client;
        _dbName = opt.Value.DatabaseName;
        _collectionName = opt.Value.LogCollectionName;

        _logs = client.GetDatabase(_dbName)
                      .GetCollection<LogEntry>(_collectionName);
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
        }
        catch (Exception ex)
        {
            info.Success = false;
            info.ErrorMessage = ex.Message;
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
                        f.Regex("RenderedMessage", new BsonRegularExpression(q.UserId, "i")));
        }

        if (q.StartDate.HasValue) filters.Add(f.Gte(l => l.Timestamp, q.StartDate.Value));
        if (q.EndDate.HasValue) filters.Add(f.Lte(l => l.Timestamp, q.EndDate.Value));

        if (!string.IsNullOrWhiteSpace(q.Method))
        {
            var m = q.Method.ToUpper();
            filters.Add(f.Eq("Properties.Method", m) | f.Eq("Properties.RequestMethod", m));
        }

        if (!string.IsNullOrWhiteSpace(q.Description))
        {
            filters.Add(f.Regex("Properties.Description", new BsonRegularExpression(q.Description, "i")) |
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

        return logs.GroupBy(l => l.ActionDescription ?? "Unknown")
                   .Select(g => new OperationTypeCount
                   {
                       Description = g.Key,
                       Count = g.Count()
                   })
                   .OrderByDescending(x => x.Count)
                   .ToList();
    }
}
