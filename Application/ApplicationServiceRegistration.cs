using Application.Pipelines.Validation;
using Application.Rules;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

// Bu dosya, Application katmanındaki servisleri ASP.NET Core DI container'ına kaydeden
// extension method sınıfını barındırır.
// "Extension Method": IServiceCollection sınıfına Program.cs'i düzenlemeden yeni metot ekler.
// Clean Architecture gereği her katman kendi DI kayıtlarını kendisi yönetir.

/// <summary>
/// Application katmanı servislerini DI container'ına kaydeden static extension sınıfı.
/// Program.cs'te builder.Services.AddApplicationServices() ile çağrılır.
/// </summary>
public static class ApplicationServiceRegistration
{
    /// <summary>
    /// MediatR, AutoMapper ve Business Rules sınıflarını DI container'ına kaydeder.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // FluentValidation: Tüm AbstractValidator<T> sınıflarını otomatik tarar ve kaydeder.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR: CQRS deseninde Command/Query nesnelerini ilgili Handler'ına yönlendirir.
        // Assembly.GetExecutingAssembly(): Application.dll içindeki tüm IRequestHandler<,> sınıflarını otomatik bulur.
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>)); // MediatR pipeline'ına doğrulama davranışı ekler.
        });

        // AutoMapper: Entity ↔ DTO ↔ Command/Response dönüşümlerini profil sınıfları üzerinden yönetir.
        // AddMaps: tüm MappingProfile türevlerini aynı assembly'de otomatik tarar.
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

        // BaseBusinessRules'ı miras alan tüm kural sınıflarını otomatik Scoped olarak kaydeder.
        // Örn: DrugBusinessRules, CustomerBusinessRules, vs.
        services.AddApplicationRules(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        return services;
    }

    #region Private Methods

    /// <summary>
    /// Verilen assembly içinde belirtilen temel türün tüm alt sınıflarını DI container'ına kaydeder.
    /// addWithLifeCycle null ise varsayılan yaşam döngüsü Scoped'tur (istek başına bir nesne).
    /// </summary>
    /// <param name="services">DI servis koleksiyonu.</param>
    /// <param name="assembly">Taranacak assembly.</param>
    /// <param name="type">Alt sınıfları aranacak temel tür (örn. BaseBusinessRules).</param>
    /// <param name="addWithLifeCycle">Farklı yaşam döngüsü için özel kayıt fonksiyonu (opsiyonel).</param>
    private static IServiceCollection AddApplicationRules(this IServiceCollection services, Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        // Reflection ile assembly'deki tüm sınıflar taranarak BaseBusinessRules'tan türeyen sınıflar bulunur.
        // t.IsSubclassOf(type): kalıtım hiyerarşisinde type'ın altında olan sınıflar
        // type != t: temel sınıfın kendisi dahil edilmez
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();

        foreach (var item in types)
        {
            if (addWithLifeCycle != null)
            {
                // Parametre olarak geçilen özel yaşam döngüsü fonksiyonunu kullan
                addWithLifeCycle(services, item);
            }
            else
            {
                // Scoped: aynı HTTP isteği içinde tek nesne oluşturulur, istek bitince yok edilir
                services.AddScoped(item);
            }
        }
        return services;
    }

    #endregion
}
