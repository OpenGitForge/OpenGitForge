using Microsoft.Extensions.Configuration;

namespace OFG.Host.Module;

public interface IModule
{
    public void Configure(IConfiguration configuration);

    public void LoadDependencies(ModuleLoader loader);
}