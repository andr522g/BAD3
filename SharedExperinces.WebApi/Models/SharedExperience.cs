using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("SharedExperience")]
public class SharedExperience
{
    [Key]
    public int SharedExperienceId { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<SharedExperienceService> ServiceSharedExperiences { get; set; }
}