using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Guest")]
public class Guest
{
    [Key]
    public int GuestId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Number { get; set; }

    // Many to many relationship
    public ICollection<SharedExperienceGuest> SharedExperienceGuest { get; set; }

    // One to many relationship
    public ICollection<Registration> Registration { get; set; }
}

