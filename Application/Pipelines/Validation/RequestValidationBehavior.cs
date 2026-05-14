using CrossCuttingConcerns.Exceptions.Types;
using FluentValidation;
using MediatR;

namespace Application.Pipelines.Validation;

// Bu dosya, MediatR Pipeline Behavior'ı kullanarak her Command/Query işlenmeden önce
// otomatik doğrulama (validation) çalıştıran ara katmanı barındırır.
//
// 🔄 MediatR Pipeline Behavior Nedir?
//   MediatR'da bir Command/Query gönderildiğinde (ISender.Send), handler çağrılmadan önce
//   araya girebileceğiniz davranışlar (behavior) tanımlayabilirsiniz.
//   Bu, ASP.NET Core Middleware'e benzer ama MediatR mesaj hattı içindir.
//
//   İstek akışı:
//   Controller → ISender.Send(command)
//              → RequestValidationBehavior (doğrulama)
//              → Handler (asıl iş mantığı)
//              → Response
//
// 🔑 Neden Validator Listesi (IEnumerable<IValidator<TRequest>>) Alıyoruz?
//   DI container, aynı Command için birden fazla Validator tanımlanmışsa hepsini listeler.
//   Örneğin CreateDrugCommandValidator ve CreateDrugCommandExtraValidator aynı anda çalışabilir.
//   Bu esneklik, büyük projelerde validation kurallarını bölmeye izin verir.

/// <summary>
/// Her MediatR isteği işlenmeden önce FluentValidation doğrulamalarını çalıştıran pipeline davranışı.
/// Doğrulama hataları varsa <see cref="ValidationException"/> fırlatır ve handler çağrılmaz.
/// DI container'a RegisterServicesFromAssembly ile kayıtlı tüm IValidator&lt;TRequest&gt; sınıfları
/// otomatik olarak bu sınıfa enjekte edilir.
/// </summary>
public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    // DI container tarafından enjekte edilen, bu TRequest türüne ait tüm validator'lar.
    // IEnumerable<IValidator<TRequest>>: birden fazla validator destekler (composite validation).
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Constructor injection: DI container bu sınıfı oluştururken validator listesini otomatik sağlar.
    /// Eğer bu TRequest için hiç validator tanımlanmamışsa liste boş gelir; hata fırlatılmaz.
    /// </summary>
    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Pipeline'daki asıl çalışma metodu. Handler çağrılmadan önce tüm validator'ları koşturur.
    /// </summary>
    /// <param name="request">İşlenecek Command veya Query nesnesi.</param>
    /// <param name="next">Bir sonraki pipeline adımı (genellikle asıl Handler).</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // ValidationContext<TRequest>: FluentValidation'ın doğrulama bağlamı.
        // Hangi nesnenin doğrulandığını validator'a bildirir.
        // TRequest kullanmak, object yerine tip güvenli (type-safe) bir bağlam sağlar.
        ValidationContext<TRequest> context = new(request);

        // Her validator çalıştırılır, sonuçlar düzleştirilerek (SelectMany) hata listesi elde edilir.
        // GroupBy: aynı alana (PropertyName) ait hatalar bir arada gruplanır.
        // Örnek çıktı: { PropertyName: "Email", ErrorMessages: ["Boş olamaz", "Geçersiz format"] }
        IEnumerable<ValidationExceptionModel> failures = _validators
            .Select(validator => validator.Validate(context))      // Her validator'ı çalıştır
            .SelectMany(result => result.Errors)                   // Tüm ValidationFailure'ları düzleştir
            .Where(failure => failure != null)                     // Null hataları filtrele (güvenlik)
            .GroupBy(
                keySelector: failure => failure.PropertyName,      // Alan adına göre grupla
                resultSelector: (propertyName, failures) => new ValidationExceptionModel
                {
                    PropertyName = propertyName,
                    ErrorMessages = failures.Select(failure => failure.ErrorMessage)
                }).ToList();

        // Herhangi bir doğrulama hatası varsa ValidationException fırlatılır.
        // Bu exception, ExceptionMiddleware tarafından yakalanıp HTTP 400 yanıtına dönüştürülür.
        // Handler (next()) HİÇ çağrılmaz — geçersiz veri asla iş mantığına ulaşmaz.
        if (failures.Any())
        {
            throw new CrossCuttingConcerns.Exceptions.Types.ValidationException(failures);
        }

        // Tüm doğrulamalar geçtiyse bir sonraki pipeline adımını (Handler) çağır.
        return await next();
    }
}
