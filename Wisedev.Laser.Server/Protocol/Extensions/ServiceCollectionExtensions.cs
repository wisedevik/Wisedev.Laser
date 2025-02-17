using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Wisedev.Laser.Server.Protocol.Attributes;

namespace Wisedev.Laser.Server.Protocol.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes()
                                  .Where(t => t.GetCustomAttribute<ServiceNodeAttribute>() != null);

        foreach (Type type in types)
        {
            services.AddScoped(type);
        }

        return services;
    }
}
