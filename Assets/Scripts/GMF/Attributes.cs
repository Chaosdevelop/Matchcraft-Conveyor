using System;
using Microsoft.Extensions.DependencyInjection;

namespace GMF
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceDescriptorAttribute : Attribute
    {
        public ServiceDescriptorAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }

        public ServiceLifetime Lifetime { get; }
    }
}