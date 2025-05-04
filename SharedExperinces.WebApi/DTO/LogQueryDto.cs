// DTO/LogQueryDto.cs
namespace SharedExperinces.WebApi.DTO;

public class LogQueryDto
{
    public string? UserName { get; init; }          //    ?userName=jane@foo.com
    public DateTime? FromUtc { get; init; }          //    ?fromUtc=2025-05-01T00:00:00Z
    public DateTime? ToUtc { get; init; }          //    ?toUtc=2025-05-03T23:59:59Z
    public string? Operation { get; init; }          //    ?operation=POST|PUT|DELETE
}
