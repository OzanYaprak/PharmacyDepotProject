using Application.Rules;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        services.AddApplicationRules(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        return services;
    }

    #region Private Methods
    
    private static IServiceCollection AddApplicationRules(this IServiceCollection services, Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();

        foreach (var item in types)
        {
            if (addWithLifeCycle != null)
            {
                addWithLifeCycle(services, item);
            }
            else
            {
                services.AddScoped(item);
            }
        }
        return services;
    }

    #endregion
}
