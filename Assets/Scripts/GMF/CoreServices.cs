using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


namespace GMF
{

    public static class Core
    {
        public class Services
        {
            private static IServiceProvider serviceProvider;

            static Services()
            {

                serviceProvider = new ServiceCollection()
               .AddServicesWithAttribute(Assembly.GetExecutingAssembly())
               .BuildServiceProvider();
            }

            public static T GetService<T>()
            {
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException("Service provider is not initialized.");
                }

                return serviceProvider.GetService<T>();
            }

            public static T GetRequiredService<T>()
            {
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException("Service provider is not initialized.");
                }

                return serviceProvider.GetRequiredService<T>();
            }


            public static IServiceScope CreateScope()
            {
                return serviceProvider.CreateScope();
            }
        }
    }



    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesWithAttribute(this IServiceCollection services, Assembly assembly)
        {
            var typesWithAttributes = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<ServiceDescriptorAttribute>() != null);

            foreach (var type in typesWithAttributes)
            {
                var attribute = type.GetCustomAttribute<ServiceDescriptorAttribute>();
                var lifetime = attribute?.Lifetime ?? ServiceLifetime.Transient;

                services.Add(new ServiceDescriptor(type.GetInterfaces().FirstOrDefault(), type, lifetime));
            }

            return services;
        }
    }
}
