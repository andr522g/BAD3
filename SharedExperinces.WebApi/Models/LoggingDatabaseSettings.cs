namespace SharedExperinces.WebApi.Models
{
    public class LoggingDatabaseSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "LoggingDb";
        public string LogCollectionName { get; set; } = "logs";
    }
}

