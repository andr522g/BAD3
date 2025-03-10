using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Discount")]
public class Discount
{
    [Key]
    public int DiscountId { get; set; }
    public int GuestCount { get; set; }
    public double Percentage { get; set; }

    [ForeignKey("Service")]
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}

