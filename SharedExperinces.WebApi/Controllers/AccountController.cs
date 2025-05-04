using AuthDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedExperinces.WebApi.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApiUser> _userMgr;
    private readonly IConfiguration _cfg;

    public AccountController(UserManager<ApiUser> um, IConfiguration cfg)
    { _userMgr = um; _cfg = cfg; }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApiUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userMgr.CreateAsync(user, dto.Password);

        return result.Succeeded ? StatusCode(201) :
            BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var token = BuildJwt(user);
        return Ok(token);
    }


    string BuildJwt(ApiUser user)
    {
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["JWT:Key"])),
            SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) };

        var jwt = new JwtSecurityToken(
            issuer: _cfg["JWT:Issuer"],
            audience: _cfg["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
