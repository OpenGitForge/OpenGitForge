using OFG.Host.Module;
using OGF.Authentication.Contracts;
using OGF.Module.Authentication.Plugin;

namespace OGF.Module.Authentication.Services;

[Injectable(typeof(IAuthenticationService))]
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAccountAuthStrategy _accountAuthStrategy;

    public AuthenticationService(IAccountAuthStrategy accountAuthStrategy)
    {
        _accountAuthStrategy = accountAuthStrategy;
    }
}