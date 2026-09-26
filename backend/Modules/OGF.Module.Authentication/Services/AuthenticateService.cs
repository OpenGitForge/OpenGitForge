using OFG.Host.Module;
using OGF.Authentication.Contracts;
using OGF.Module.Authentication.EP;

namespace OGF.Module.Authentication.Services;

[Injectable(typeof(IAuthenticationService))]
public sealed class AuthenticateService : IAuthenticationService
{
    private readonly IAuthStrategy _authStrategy;

    public AuthenticateService(IAuthStrategy authStrategy)
    {
        _authStrategy = authStrategy;
    }
}