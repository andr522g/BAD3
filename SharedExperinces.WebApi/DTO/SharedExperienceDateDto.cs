namespace SharedExperinces.WebApi.DTO
{
    public class SharedExperienceDateDto
    {
        public int SharedExperienceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? EarliestServiceDate { get; set; }
    }
}
