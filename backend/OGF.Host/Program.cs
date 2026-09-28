using System.Reflection;
using Microsoft.Extensions.Options;
using OFG.Host.Module;
using Scalar.AspNetCore;
using Tomlyn.Extensions.Configuration;

namespace OGF.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        LoadConfigurationFiles(builder);
        ModuleLoader loader = LoadModules(builder);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        foreach (Assembly module in loader.Modules)
        {
            builder.Services.AddMvc().AddApplicationPart(module).AddControllersAsServices();
            DiRegistry.RegisterServices(module);   
        }
        
        DiRegistry.BindServices(builder.Services);

        WebApplication app = builder.Build();

        app.MapOpenApi();
        app.MapScalarApiReference(options => { options.WithTitle("OpenGitForge"); });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void LoadConfigurationFiles(WebApplicationBuilder builder)
    {

        builder.Configuration.AddTomlFile("config/host.toml", optional: false);
        foreach (string file in Directory.GetFiles("config/modules", "*.toml", SearchOption.AllDirectories))
        {
            builder.Configuration.AddTomlFile(file, optional: true);
        }

        foreach (string file in Directory.GetFiles("config/plugins", "*.toml", SearchOption.AllDirectories))
        {
            builder.Configuration.AddTomlFile(file, optional: true);
        }

        builder.Services.AddOptions<HostOptions>()
            .Bind(builder.Configuration.GetSection("Host"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static ModuleLoader LoadModules(WebApplicationBuilder builder)
    {
        IServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<HostOptions> options = provider.GetRequiredService<IOptions<HostOptions>>();
        ModuleLoader loader = new ModuleLoader(builder.Services, builder.Configuration);

        foreach (string module in options.Value.Modules)
        {
            loader.LoadModule(module);
        }

        return loader;
    }
}