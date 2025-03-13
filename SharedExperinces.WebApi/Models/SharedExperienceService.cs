namespace SharedExperinces.WebApi.Models
{
    public class SharedExperienceService
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int SharedExperienceId { get; set; }
        public SharedExperience SharedExperience { get; set; }
    }
}
