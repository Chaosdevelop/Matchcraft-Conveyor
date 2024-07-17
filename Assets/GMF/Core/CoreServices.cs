using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


namespace GMF
{

    public static class Core
    {

        public class Services
        {


            static IServiceProvider serviceProvider;


            public static void Initialize(IEnumerable<ServiceDescriptor> serviceDescriptors = null)
            {

                var collection = new ServiceCollection();
                if (serviceDescriptors != null)
                {
                    foreach (var serviceDescriptor in serviceDescriptors)
                    {
                        (collection as IServiceCollection).Add(serviceDescriptor);
                    }
                }

                collection.AddServicesWithAttribute();
                serviceProvider = collection.BuildServiceProvider();
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


        public static IServiceCollection AddServicesWithAttribute(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {

                var typesWithAttributes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<ServiceDescriptorAttribute>() != null);


                foreach (var type in typesWithAttributes)
                {
                    var attribute = type.GetCustomAttribute<ServiceDescriptorAttribute>();
                    var lifetime = attribute?.Lifetime ?? ServiceLifetime.Transient;

                    services.Add(new ServiceDescriptor(type.GetInterfaces().FirstOrDefault(), type, lifetime));
                }
            }


            return services;
        }
    }
}
