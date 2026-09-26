using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace OFG.Host.Module;

public sealed class ModuleLoader
{
    private readonly IConfiguration _configuration;

    public ModuleLoader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void LoadModule(string assemblyFile)
    {
        Assembly assembly = Assembly.Load(assemblyFile);
        LoadModule(assembly);
    }

    public void LoadModule(Assembly assembly)
    {
        Type[] moduleTypes = assembly.GetTypes().Where(type =>
            type.IsClass &&
            !type.IsAbstract &&
            type.IsAssignableTo(typeof(IModule))
        ).ToArray();

        foreach (Type moduleType in moduleTypes)
        {
            IModule? module = (IModule?)Activator.CreateInstance(moduleType);
            if (module is null)
            {
                throw new InvalidOperationException($"Failed to create module instance '{moduleType.FullName}'");
            }

            LoadModule(module);
        }
    }

    public void LoadModule(IModule module)
    {
        module.Configure(_configuration);
        module.LoadDependencies(this);
    }
}