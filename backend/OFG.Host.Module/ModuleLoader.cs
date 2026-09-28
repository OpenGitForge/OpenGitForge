using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OFG.Host.Module;

public sealed class ModuleLoader
{
    private readonly HashSet<string> _loadedAssemblies;
    private readonly IServiceCollection _service;
    private readonly IConfiguration _configuration;

    public ModuleLoader(IServiceCollection service, IConfiguration configuration)
    {
        _loadedAssemblies = new HashSet<string>();
        _service = service;
        _configuration = configuration;
    }

    public void LoadModule(string assemblyFile)
    {
        string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, assemblyFile));

        Assembly assembly = Assembly.LoadFile(path);
        if (_loadedAssemblies.Add(assembly.Location))
        {
            LoadModule(assembly);
        }
    }

    public OptionsBuilder<TOptions> AddOptions<TOptions>(IConfigurationSection section) where TOptions : class =>
        _service.AddOptions<TOptions>()
            .Bind(section);

    public void LoadModule(Assembly assembly)
    {
        Type[] moduleTypes = assembly.GetTypes().Where(type =>
            type.IsClass &&
            !type.IsAbstract &&
            type.IsAssignableTo(typeof(IModule))
        ).ToArray();

        if (moduleTypes.Length == 0)
        {
            throw new InvalidOperationException($"No modules found in assembly '{assembly.FullName}'");
        }

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
        module.Configure(this, _configuration);
        module.LoadDependencies(this);
    }
}