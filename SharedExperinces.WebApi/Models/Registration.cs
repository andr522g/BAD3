using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Registration")]
public class Registration
{
    [Key]
    public int RegistrationId { get; set; }

    // Many to one relationship
    public ICollection<Service> Service { get; set; }

    // Many to one relationship
    public ICollection<Guest> Guest { get; set; }
}

