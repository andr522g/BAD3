using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.Models;

[Route("api/[controller]")]
[ApiController]
public class DemoController : ControllerBase
{
    // 0. Truly public (no token needed)
    [HttpGet("public")]
    [AllowAnonymous]
    public string Public() => "Everyone can see this.";

    // 1. Guest  (= logged-in but *only* role Guest)
    [HttpGet("guest")]
    [Authorize(Roles = RoleNames.Guest)]
    public string GuestOnly() => "Hello Guest!";

    // 2. Provider
    [HttpGet("provider")]
    [Authorize(Roles = RoleNames.Provider)]
    public string ProviderOnly() => "Hello Provider – here are your orders.";

    // 3. Manager
    [HttpGet("manager")]
    [Authorize(Roles = RoleNames.Manager)]
    public string ManagerOnly() => "Hello Manager – KPI dashboard incoming.";

    // 4. Admin
    [HttpGet("admin")]
    [Authorize(Roles = RoleNames.Admin)]
    public string AdminOnly() => "Welcome, mighty admin.";
}
