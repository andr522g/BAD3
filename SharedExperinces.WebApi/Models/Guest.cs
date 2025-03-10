using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharedExperinces.WebApi.Models;

[Table("Guest")]
public class Guest
{
    [Key]
    public int GuestId { get; set; }
}

