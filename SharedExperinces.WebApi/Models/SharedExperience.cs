using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("SharedExperience")]
public class SharedExperience
{
    [Key]
    public int SharedExperienceId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime ExperinceDate { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<SharedExperienceService> SharedExperiencesService { get; set; }
    public ICollection<SharedExperienceGuest> SharedExperienceGuest { get; set; }
}