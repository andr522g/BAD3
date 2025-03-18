using System.Collections.Generic;

namespace SharedExperinces.WebApi.Models
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Number { get; set; }
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
		public ICollection<SharedExperience> SharedExperiences { get; set; } = new List<SharedExperience>();
	}
}
