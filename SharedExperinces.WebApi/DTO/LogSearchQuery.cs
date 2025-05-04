namespace SharedExperinces.WebApi.DTO;

public class LogSearchQuery
{
    public string? UserId { get; init; }
    public string? Method { get; init; }   // POST | PUT | DELETE
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
