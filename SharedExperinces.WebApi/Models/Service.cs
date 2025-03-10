using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Service")]
public class Service
{
    [Key]
    public int ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ServiceDate { get; set; }

    // Many-to-one relationship with Provider
    [ForeignKey("Provider")]
    public string CVR { get; set; }
    public Provider Provider { get; set; }

    // Many-to-many relationship with SharedExperience
    public ICollection<SharedExperienceService> ServiceSharedExperiences { get; set; }

    // One-to-many relationship with Discount
    public ICollection<Discount> Discount { get; set; }

    // One-to-many relationship with Registration
    public ICollection<Registration> Registration { get; set; }

}