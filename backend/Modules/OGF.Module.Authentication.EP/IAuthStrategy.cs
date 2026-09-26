namespace OGF.Module.Authentication.EP;

public interface IAuthStrategy
{
    public Task AuthenticateAsync(string username, string password);
}