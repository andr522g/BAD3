using System.Collections.Generic;

namespace SharedExperinces.WebApi.Models
{
    public class SharedExperience
    {
        public int SharedExperienceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Guest> Guests { get; set; }

        public SharedExperience() 
        {

			Services = new List<Service>();
			Guests = new List<Guest>();
		}
	}
}
