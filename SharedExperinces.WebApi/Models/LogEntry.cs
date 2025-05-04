using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Globalization;

namespace SharedExperinces.WebApi.Models
{
    [BsonIgnoreExtraElements]          // ignore anything we don’t map
    public class LogEntry
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public DateTime Timestamp { get; set; }        // written by Serilog

        // the Serilog BSON sink stores the level in a top-level “Level”
        public string Level { get; set; } = null!;

        public string MessageTemplate { get; set; } = null!;
        public string RenderedMessage { get; set; } = null!;
        public string? Exception { get; set; }

        public LogProperties Properties { get; set; } = null!;

        /* helpers ----------------------------------------------------------*/

        [BsonIgnore]
        public string ActionDescription
            => Properties.Description ?? Properties.Method ?? "Unknown";
    }

    [BsonIgnoreExtraElements]
    public class LogProperties
    {
        public string? UserId { get; set; }
        public string? UserRole { get; set; }
        public string? UserEmail { get; set; }
        public string? Method { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? Description { get; set; }
        public int? StatusCode { get; set; }
        public double? Elapsed { get; set; }
    }
}
