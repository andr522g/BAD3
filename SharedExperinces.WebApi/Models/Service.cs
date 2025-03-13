using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("Service")]
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        // Many-to-one relationship with Provider
        [Required]
        [ForeignKey(nameof(Provider))]
        public string CVR { get; set; }
        public Provider Provider { get; set; }

        // Many-to-many with SharedExperience
        public ICollection<SharedExperienceService> SharedExperienceService { get; set; }

        // One-to-many with Discount
        public ICollection<Discount> Discount { get; set; }

        // One-to-many with Registration
        public ICollection<Registration> Registration { get; set; }
    }
}
