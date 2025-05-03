using System.ComponentModel.DataAnnotations;

namespace SharedExperinces.WebApi.DTO
{
    public class RegisterDto
    {
        [Required] public string? UserName { get; set; }

        [Required] public string? Role { get; set; }


        [Required] public string? Email { get; set; }
        [Required] public string? Password { get; set; }
    }
}
