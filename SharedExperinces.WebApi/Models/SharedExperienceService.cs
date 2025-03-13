using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("SharedExperienceService")]
    public class SharedExperienceService
    {
        // Composite key: (ServiceId, SharedExperienceId)
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(SharedExperience))]
        public int SharedExperienceId { get; set; }
        public SharedExperience SharedExperience { get; set; }
    }
}
