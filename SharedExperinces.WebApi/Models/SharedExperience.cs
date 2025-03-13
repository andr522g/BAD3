using System.Collections.Generic;

namespace SharedExperinces.WebApi.Models
{
    public class SharedExperience
    {
        public int SharedExperienceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<SharedExperienceService> SharedExperiencesService { get; set; }
        public ICollection<SharedExperienceGuest> SharedExperienceGuest { get; set; }
    }
}
