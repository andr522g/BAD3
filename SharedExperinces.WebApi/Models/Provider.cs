using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Provider")]
public class Provider
{
    [Key]
    [Required]
    [MaxLength(8), MinLength(8)]
    public string CVR { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string PhoneNumber { get; set; }
}

