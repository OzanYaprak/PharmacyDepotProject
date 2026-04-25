# PharmacyDepot - Copilot Talimat Dosyası

## Proje Genel Bilgisi

Bu proje **PharmacyDepot** adlı bir eczane deposu yönetim sistemidir.
- Mimari: **Clean Architecture**
- Platform: **.NET 10**
- Dil: **C#**

---

## Katman Yapısı

Çözüm 5 projeden oluşmaktadır:

| Proje | Sorumluluk |
|---|---|
| `Domain` | Entity sınıfları, domain kuralları, arayüzler |
| `Application` | Use case'ler, CQRS komutları/sorguları, servis arayüzleri, DTO'lar |
| `Persistence` | Veritabanı bağlamı (DbContext), migration'lar, repository implementasyonları |
| `Infrastructure` | Harici servisler (e-posta, dosya sistemi, vb.) |
| `WebAPI` | Controller'lar, endpoint tanımları, DI kayıtları |

### Katman Bağımlılık Kuralı
- `Domain` → hiçbir katmana bağımlı değildir
- `Application` → yalnızca `Domain`'e bağımlıdır
- `Persistence` ve `Infrastructure` → `Application`'a bağımlıdır
- `WebAPI` → tüm katmanlara bağımlıdır (DI composition root)

---

## Kodlama Kuralları

### Genel
- Her sınıf tek bir sorumluluk taşır (SRP).
- `var` kullanımı tercih edilir, ancak tür açık ve anlaşılır olmalıdır.
- `async/await` zorunludur; `.Result` veya `.Wait()` **kullanılmaz**.
- Magic string ve magic number kullanılmaz; sabitler `const` veya `static readonly` ile tanımlanır.
- XML doc comment (`///`) yalnızca public API'lere eklenir.

### İsimlendirme
- Sınıf, interface, property, metot adları: **PascalCase**
- Yerel değişkenler, parametreler: **camelCase**
- Private field'lar: `_camelCase` (alt çizgi öneki)
- Interface'ler: `I` öneki (örn. `IDrugRepository`)
- Generic type parametreleri: `T`, `TEntity`, `TResult`

---

## Katman Bazlı Kurallar

### Domain
- `Domain\Entities\` klasörüne entity sınıfları eklenir.
- Entity'ler `BaseEntity` gibi bir taban sınıftan türetilir (varsa).
- Entity'lerde **business logic** barındırılabilir; ancak altyapı bağımlılığı **olmaz**.
- Navigation property'ler sanal (`virtual`) tanımlanır.
- Constructor'lar `private set` ile birlikte kullanılarak encapsulation sağlanır.

```csharp
// Örnek entity yapısı
namespace Domain.Entities;

public class Drug
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string ActiveIngredient { get; private set; } = default!;
    public decimal Price { get; private set; }

    private Drug() { } // EF Core için

    public Drug(string name, string activeIngredient, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        ActiveIngredient = activeIngredient;
        Price = price;
    }
}
```

### Application
- CQRS deseni kullanılır: her işlem için ayrı **Command** veya **Query** nesnesi tanımlanır.
- Klasör yapısı: `Application\Features\{Entity}\{Commands|Queries}\{OperationName}\`
- Her Command/Query kendi `Handler`'ını içerir.
- MediatR ile handler'lar register edilir.
- Validation için **FluentValidation** kullanılır; her Command için ayrı `Validator` sınıfı yazılır.
- Repository'ler interface aracılığıyla kullanılır (`IRepository<T>` veya entity bazlı arayüzler).
- DTO'lar `Application\Features\{Entity}\Dtos\` altında tutulur.

```csharp
// Örnek Command yapısı
namespace Application.Features.Drugs.Commands.CreateDrug;

public record CreateDrugCommand(string Name, string ActiveIngredient, decimal Price)
    : IRequest<Guid>;

public sealed class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, Guid>
{
    private readonly IDrugRepository _drugRepository;

    public CreateDrugCommandHandler(IDrugRepository drugRepository)
        => _drugRepository = drugRepository;

    public async Task<Guid> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        var drug = new Drug(request.Name, request.ActiveIngredient, request.Price);
        await _drugRepository.AddAsync(drug, cancellationToken);
        return drug.Id;
    }
}
```

### Persistence
- `DbContext` sınıfı `Persistence\Contexts\` altında yer alır.
- Her entity için `IEntityTypeConfiguration<T>` implementasyonu `Persistence\Configurations\` altında yazılır.
- Repository implementasyonları `Persistence\Repositories\` altında tutulur.
- Migration'lar otomatik oluşturulur; elle düzenlenmez.

### WebAPI
- Controller'lar `WebAPI\Controllers\` altında yer alır.
- Controller'lar yalnızca MediatR `ISender` kullanır; doğrudan servis çağrısı yapılmaz.
- Route isimleri **kebab-case** ve **çoğul** kullanılır: `/api/drugs`
- HTTP durum kodları anlamlı döndürülür (`200`, `201`, `204`, `400`, `404`, `422`).

```csharp
// Örnek Controller yapısı
[ApiController]
[Route("api/[controller]")]
public class DrugsController : ControllerBase
{
    private readonly ISender _sender;

    public DrugsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateDrugCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
```

---

## Hata Yönetimi

- Domain hataları için özel exception sınıfları `Domain\Exceptions\` altında tanımlanır.
- `Application` katmanında `Result<T>` pattern'i veya exception fırlatma tercih edilir (projede hangisi kullanılıyorsa tutarlı kalınır).
- Global exception handler middleware `WebAPI` katmanında bulunur.

---

## Test Kuralları (İleride Eklenirse)

- Unit testler `Application` ve `Domain` katmanlarını kapsar.
- Integration testler `Persistence` ve `WebAPI` katmanlarını kapsar.
- Test sınıfı isimlendirme: `{SınıfAdı}Tests`
- Test metot isimlendirme: `{Metot}_{Durum}_{BeklenenSonuç}`

---

## Yeni Bir Entity Eklerken Yapılacaklar (Checklist)

1. `Domain\Entities\` → Entity sınıfını oluştur.
2. `Domain\Interfaces\` (veya `Application\Interfaces\`) → Repository arayüzünü tanımla.
3. `Application\Features\{Entity}\` → CQRS Command/Query'lerini yaz.
4. `Persistence\Configurations\` → EF Core konfigürasyonunu ekle.
5. `Persistence\Repositories\` → Repository implementasyonunu yaz.
6. `Persistence\Contexts\DbContext.cs` → `DbSet<T>` ekle.
7. `WebAPI\Controllers\` → Controller'ı oluştur.
8. `WebAPI\Program.cs` → Gerekli DI kayıtlarını yap.
9. Migration oluştur: `dotnet ef migrations add {MigrationAdı}`

---

## Teknoloji Yığını

- **ORM:** Entity Framework Core
- **Mediator:** MediatR
- **Validation:** FluentValidation
- **Mapping:** AutoMapper veya manuel mapping (tutarlı olunacak)
- **Veritabanı:** (projede belirlenen DB — örn. SQL Server / PostgreSQL)
