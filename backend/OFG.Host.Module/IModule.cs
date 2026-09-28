using Microsoft.Extensions.Configuration;

namespace OFG.Host.Module;

public interface IModule
{
    public void Configure(ModuleLoader loader, IConfiguration configuration);

    public void LoadDependencies(ModuleLoader loader);
}