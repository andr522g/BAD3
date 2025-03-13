using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("Registration")]
    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }

        // Many-to-one relationship
        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        [ForeignKey(nameof(Guest))]
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
    }
}
