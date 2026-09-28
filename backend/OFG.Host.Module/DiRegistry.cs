using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OFG.Host.Module;

public static class DiRegistry
{
    private static readonly Dictionary<Type, InjectableAttribute> _services = new Dictionary<Type, InjectableAttribute>();

    public static void RegisterServices(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
            {
                continue;
            }

            InjectableAttribute? attribute = type.GetCustomAttribute<InjectableAttribute>();
            if (attribute is null)
            {
                continue;
            }
            
            RegisterService(type, attribute);
        }
    }
    
    public static void RegisterService(Type implementationType, InjectableAttribute attribute)
    {
        Type contractType = attribute.ContractType is null
            ? implementationType
            : attribute.ContractType;
        if (!implementationType.IsAssignableTo(contractType))
        {
            throw new InvalidOperationException(
                $"Implementation type '{implementationType.FullName}' is not assignable to contract type '{contractType.FullName}'");
        }

        if (!_services.TryAdd(implementationType, attribute))
        {
            throw new InvalidOperationException($"Service '{contractType.FullName}' is already registered");
        }
    }

    public static void BindServices(IServiceCollection collection)
    {
        foreach ((Type implementationType, InjectableAttribute attribute) in _services)
        {
            collection.Add(new ServiceDescriptor(
                attribute.ContractType ?? implementationType,
                implementationType,
                attribute.Lifetime
            ));
        }
    }
}