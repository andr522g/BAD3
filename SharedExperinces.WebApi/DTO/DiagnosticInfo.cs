namespace SharedExperinces.WebApi.DTO;

public class DiagnosticInfo
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? DatabaseName { get; set; }
    public string? CollectionName { get; set; }
    public long TotalLogCount { get; set; }
    public bool HasSampleLog { get; set; }
    public string? SampleLogId { get; set; }
    public DateTime? SampleLogTime { get; set; }
    public string? SampleLogMessage { get; set; }
    public bool HasUserIdField { get; set; }
    public string? SampleUserId { get; set; }
    public string? RawDocumentStructure { get; set; }
}
