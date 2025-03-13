using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        // e.g., must be at least 1 and not insanely large
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "GuestCount must be a positive integer.")]
        public int GuestCount { get; set; }

        // if percentage is 0–100
        [Required]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        public double Percentage { get; set; }

        // One-to-many relationship with Service
        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
