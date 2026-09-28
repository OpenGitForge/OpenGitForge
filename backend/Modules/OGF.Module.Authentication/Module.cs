using OFG.Host.Module;

namespace OGF.Module.Authentication;

public sealed class Module : IModule
{
    public void Configure(ModuleLoader loader, IConfiguration configuration)
    {
        loader.AddOptions<AuthenticationOptions>(configuration.GetSection("Modules").GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public void LoadDependencies(ModuleLoader loader)
    {
        
    }
}