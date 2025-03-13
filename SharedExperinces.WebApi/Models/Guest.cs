using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("Guest")]
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // e.g. allow age between 0 and 120
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        [Phone] // or just [MaxLength(...)] if not strictly phone
        public string Number { get; set; }

        // Many-to-many relationship
        public ICollection<SharedExperienceGuest> SharedExperienceGuest { get; set; }

        // One-to-many relationship
        public ICollection<Registration> Registration { get; set; }
    }
}
