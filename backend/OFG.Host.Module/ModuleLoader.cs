using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OFG.Host.Module;

public sealed class ModuleLoader
{
    private readonly List<Assembly> _modules;
    private readonly List<Assembly> _plugins;
    private readonly HashSet<string> _loadedAssemblies;
    private readonly IServiceCollection _service;
    private readonly IConfiguration _configuration;

    public IReadOnlyList<Assembly> Modules => _modules;
    public IReadOnlyList<Assembly> Plugins => _plugins;
    
    public ModuleLoader(IServiceCollection service, IConfiguration configuration)
    {
        _modules = new List<Assembly>();
        _plugins = new List<Assembly>();
        _loadedAssemblies = new HashSet<string>();
        _service = service;
        _configuration = configuration;
    }

    public OptionsBuilder<TOptions> AddOptions<TOptions>(IConfigurationSection section) where TOptions : class =>
        _service.AddOptions<TOptions>()
            .Bind(section);

    public TOptions LoadOptions<TOptions>() where TOptions : class
    {
        IServiceProvider provider = _service.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<TOptions>>().Value;
    }

    public void LoadModule(string assemblyFile)
    {
        string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, assemblyFile));

        Assembly assembly = Assembly.LoadFile(path);
        if (_loadedAssemblies.Add(assembly.Location))
        {
            _modules.Add(assembly);
            LoadModule(assembly);
        }
    }

    private void LoadModule(Assembly assembly)
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

    private void LoadModule(IModule module)
    {
        module.Configure(this, _configuration);
        module.LoadDependencies(this);
    }

    public void LoadPlugin(string assemblyName)
    {
        string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, assemblyName));
        Assembly assembly = Assembly.LoadFile(path);
        if (_loadedAssemblies.Add(assembly.Location))
        {
            _plugins.Add(assembly);
            LoadPlugin(assembly);
        }
    }

    private void LoadPlugin(Assembly assembly)
    {
        // Doing nothing for now
    }
}