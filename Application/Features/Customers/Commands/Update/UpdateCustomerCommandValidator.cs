using FluentValidation;

namespace Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
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
