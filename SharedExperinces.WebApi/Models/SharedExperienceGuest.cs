namespace SharedExperinces.WebApi.Models
{
    public class SharedExperienceGuest
    {
        public int GuestId { get; set; }
        public Guest Guest { get; set; }

        public int SharedExperienceId { get; set; }
        public SharedExperience SharedExperience { get; set; }
    }
}
