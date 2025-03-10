using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Registration")]
public class Registration
{
    [Key]
    public int RegistrationId { get; set; }
}

