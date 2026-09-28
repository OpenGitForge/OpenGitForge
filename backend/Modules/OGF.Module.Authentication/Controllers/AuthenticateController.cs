using Microsoft.AspNetCore.Mvc;
using OGF.Authentication.Contracts;

namespace OGF.Module.Authentication.Controllers;

[ApiController]
[Route("authentication/authenticate")]
public sealed class AuthenticateController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticateController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Authenticate()
    {
        return NoContent();
    }
}