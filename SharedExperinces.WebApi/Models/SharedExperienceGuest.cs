using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("SharedExperienceGuest")]
    public class SharedExperienceGuest
    {
        // Composite key: (GuestId, SharedExperienceId)
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Guest))]
        public int GuestId { get; set; }
        public Guest Guest { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(SharedExperience))]
        public int SharedExperienceId { get; set; }
        public SharedExperience SharedExperience { get; set; }
    }
}
