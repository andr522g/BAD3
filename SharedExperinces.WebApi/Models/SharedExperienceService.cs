using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("SharedExperienceService")]
public class SharedExperienceService
{
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public int SharedExperienceId { get; set; }
    public SharedExperience SharedExperience { get; set; }
}







