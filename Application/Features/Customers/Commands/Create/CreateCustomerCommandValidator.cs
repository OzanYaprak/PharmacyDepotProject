using FluentValidation;

namespace Application.Features.Customers.Commands.Create;

// Bu dosya, müşteri oluşturma komutunun (CreateCustomerCommand) doğrulama kurallarını barındırır.
//
// 🔑 FluentValidation Temel Kavramları:
//
//   AbstractValidator<T>:
//     Tüm validator sınıfları bu sınıftan türer. T = doğrulanacak nesnenin türü.
//
//   RuleFor(x => x.PropertyName):
//     Hangi alan için kural tanımlandığını belirtir. Lambda ifadesiyle tip güvenli erişim sağlanır.
//
//   .NotEmpty():
//     Alanın null, boş string ("") veya yalnızca boşluk karakteri içermemesini zorunlu kılar.
//
//   .MaximumLength(n):
//     Alanın en fazla n karakter uzunluğunda olmasını zorunlu kılar.
//
//   .EmailAddress():
//     Alanın geçerli bir e-posta formatında olmasını kontrol eder.
//
//   .WithMessage("..."):
//     Kural ihlal edildiğinde döndürülecek hata mesajını özelleştirir.
//     Kullanılmazsa FluentValidation'ın varsayılan İngilizce mesajı kullanılır.
//
// 🔄 Bu validator nasıl devreye giriyor?
//   1. Controller → ISender.Send(new CreateCustomerCommand(...))
//   2. MediatR Pipeline → RequestValidationBehavior çalışır
//   3. RequestValidationBehavior → CreateCustomerCommandValidator.Validate() çağrılır
//   4. Hata varsa → ValidationException fırlatılır, Handler çağrılmaz
//   5. Hata yoksa → CreateCustomerCommandHandler çalışır

/// <summary>
/// <see cref="CreateCustomerCommand"/> için FluentValidation doğrulama kurallarını tanımlar.
/// MediatR pipeline'ı aracılığıyla komut işlenmeden önce otomatik olarak çalıştırılır.
/// </summary>
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // Ad alanı: zorunlu ve en fazla 100 karakter
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz.")
            .MaximumLength(100).WithMessage("Müşteri adı 100 karakteri geçemez.");

        // Ruhsat numarası: zorunlu ve en fazla 50 karakter
        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("Ruhsat numarası boş olamaz.")
            .MaximumLength(50).WithMessage("Ruhsat numarası 50 karakteri geçemez.");

        // Telefon: zorunlu (format doğrulaması eklenebilir, örn. .Matches(@"^\+?[0-9]{10,13}$"))
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.");

        // E-posta: zorunlu ve geçerli format
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        // Adres: zorunlu
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş olamaz.");
    }
}
