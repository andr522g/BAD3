using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DemoController : ControllerBase
{
    //  1. Public
    [HttpGet("public")]
    [AllowAnonymous]
    public string Public() => "Everyone can see this.";

    //  2. Authenticated
    [HttpGet("user")]
    [Authorize]                       // any logged-in user
    public string UserOnly() => "Hello registered user!";

    //  3. Admin
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]      // only role Admin
    public string AdminOnly() => "Welcome, mighty admin.";
}
