namespace SharedExperinces.WebApi.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }        
    }
}
