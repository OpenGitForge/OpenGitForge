using Microsoft.Extensions.DependencyInjection;
using OFG.Host.Module;

namespace OGF.Module.Authentication.Plugin.Default;

[Injectable(typeof(IAccountAuthStrategy), ServiceLifetime.Singleton)]
public sealed class AccountAuthStrategy : IAccountAuthStrategy
{
    public string AuthenticateAccount(string username, string password)
    {
        return "Authenticated";
    }
}