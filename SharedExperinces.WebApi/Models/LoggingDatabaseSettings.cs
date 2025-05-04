namespace SharedExperinces.WebApi.Models
{
    public class LoggingDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string LogCollectionName { get; set; } = null!;
    }
}

