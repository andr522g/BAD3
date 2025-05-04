using AuthDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;
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

    
    [HttpPost("register/guest")]
	[AllowAnonymous]

	public async Task<IActionResult> RegisterGuest(RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApiUser { UserName = dto.Email, Email = dto.Email,  };
        var result = await _userMgr.CreateAsync(user, dto.Password);


        if (result.Succeeded)
        {
			await _userMgr.AddToRoleAsync(user, RoleNames.Guest);
		}


		return result.Succeeded ? StatusCode(201) :
            BadRequest(result.Errors);
    }



	[HttpPost("register")]
	[Authorize(Roles = RoleNames.Admin)]
	public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model)
	{

		if (model.Role == RoleNames.Guest)
			return BadRequest("Guests must self-register.");

		var user = new ApiUser { UserName = model.UserName, Email = model.Email };

		var result = await _userMgr.CreateAsync(user, model.Password);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		await _userMgr.AddToRoleAsync(user, model.Role);

		return Ok($"{model.Role} user registered successfully.");

	}


	[HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var token = await BuildJwt(user);
        return Ok(token);
    }


    async Task<string> BuildJwt(ApiUser user)
    {
		var roles = await _userMgr.GetRolesAsync(user);

		var creds = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["JWT:Key"])),
			SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, user.UserName)
	};

		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var jwt = new JwtSecurityToken(
            issuer: _cfg["JWT:Issuer"],
            audience: _cfg["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
