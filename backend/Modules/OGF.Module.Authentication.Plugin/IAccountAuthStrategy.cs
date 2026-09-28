namespace OGF.Module.Authentication.Plugin;

public interface IAccountAuthStrategy
{
    public string AuthenticateAccount(string username, string password);
}
