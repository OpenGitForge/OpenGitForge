using OFG.Host.Module;
using OGF.Authentication.Contracts;
using OGF.Module.Authentication.EP;

namespace OGF.Module.Authentication.Services;

[Injectable(typeof(IAuthenticationService))]
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAuthStrategy _authStrategy;

    public AuthenticationService(IAuthStrategy authStrategy)
    {
        _authStrategy = authStrategy;
    }
}