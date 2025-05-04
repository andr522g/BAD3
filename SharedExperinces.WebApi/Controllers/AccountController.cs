using AuthDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedExperinces.WebApi.Controllers;
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
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<ApiUser> um, IConfiguration cfg, ILogger<AccountController> logger)
    { 
        _userMgr = um; 
        _cfg = cfg;
        _logger = logger;
    }


	[Authorize(Roles = "Admin")]
	[HttpPost("register")]
	public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto)
	{

		if (dto.Role == RoleNames.Guest)
			return BadRequest("Guests must self-register.");

		using (Serilog.Context.LogContext.PushProperty("RequestMethod", "POST"))
		{
			// ─── 2.  write the log entry without the password ──────────────────────
			_logger.LogInformation("Register for {Email}", dto.Email);
		}

		var user = new ApiUser { UserName = dto.UserName, Email = dto.Email };

		var result = await _userMgr.CreateAsync(user, dto.Password);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		await _userMgr.AddToRoleAsync(user, dto.Role);

		return Ok($"{dto.Role} user registered successfully.");

	}



	[HttpPost("register/guest")]
	[AllowAnonymous]

	public async Task<IActionResult> RegisterGuest(RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // ─── 1.  (optional) enrich so we survive the RequestMethod filter ──────────
        using (Serilog.Context.LogContext.PushProperty("RequestMethod", "POST"))
        {
            // ─── 2.  write the log entry without the password ──────────────────────
            _logger.LogInformation("RegisterGuest for {Email}", dto.Email);
        }

        var user = new ApiUser { UserName = dto.Email, Email = dto.Email,  };
        var result = await _userMgr.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
			await _userMgr.AddToRoleAsync(user, RoleNames.Guest);
		}


		return result.Succeeded ? StatusCode(201) :
            BadRequest(result.Errors);
    }



	


	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        using (Serilog.Context.LogContext.PushProperty("RequestMethod", "POST"))
        {
            // ─── 2.  write the log entry without the password ──────────────────────
            _logger.LogInformation("Login for {Email}", dto.Email);
        }

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
		new Claim(ClaimTypes.Name, user.UserName),

		 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		 new Claim(ClaimTypes.NameIdentifier, user.Id)
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
