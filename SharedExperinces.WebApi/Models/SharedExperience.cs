using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("SharedExperience")]
    public class SharedExperience
    {
        [Key]
        public int SharedExperienceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public DateTime ExperinceDate { get; set; }

        // Many-to-many relationship
        public ICollection<SharedExperienceService> SharedExperiencesService { get; set; }
        public ICollection<SharedExperienceGuest> SharedExperienceGuest { get; set; }
    }
}
