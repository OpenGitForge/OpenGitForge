using Microsoft.Extensions.DependencyInjection;

namespace OFG.Host.Module;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InjectableAttribute : Attribute
{
    public Type? ContractType { get; }
    public ServiceLifetime Lifetime { get; }

    public InjectableAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ContractType = null;
        Lifetime = lifetime;
    }

    public InjectableAttribute(Type contractType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ContractType = contractType;
        Lifetime = lifetime;
    }
}