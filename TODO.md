# PharmacyDepot - Yapilacaklar Listesi

## Durum Gostergesi
- [ ] Yapilmadi
- [x] Tamamlandi

---

## ASAMA 1 - NuGet Paketleri

### Application Projesi
- [x] `MediatR` paketini ekle *(14.1.0)*
- [ ] `FluentValidation` paketini ekle
- [x] `AutoMapper` paketini ekle
- [ ] `FluentValidation.DependencyInjectionExtensions` paketini ekle

### Persistence Projesi
- [x] `Microsoft.EntityFrameworkCore` paketini ekle
- [x] `Microsoft.EntityFrameworkCore.SqlServer` paketini ekle *(veya Npgsql.EntityFrameworkCore.PostgreSQL)*
- [x] `Microsoft.EntityFrameworkCore.Tools` paketini ekle
- [x] `Microsoft.EntityFrameworkCore.Design` paketini ekle

### WebAPI Projesi
- [x] `Microsoft.EntityFrameworkCore.Design` paketini ekle *(migration icin)*

---

## ASAMA 2 - Domain Katmani

### 2.1 Entity'ler `Domain\Entities\`
- [x] `Base\BaseEntity.cs` *(mevcut)*
- [x] `Interfaces\IEntityTimeStamps.cs` *(mevcut)*
- [x] `Drug.cs` *(mevcut)*
- [x] `Supplier.cs` - Tedarikci firmasi *(mevcut)*
- [x] `Warehouse.cs` - Depo / raf bilgisi
- [x] `Stock.cs` - Ilac stok hareketi
- [x] `Customer.cs` - Musteri eczane
- [x] `Order.cs` - Satin alma siparisi
- [x] `OrderItem.cs` - Siparis kalemi
- [x] `Sale.cs` - Satis kaydi
- [x] `SaleItem.cs` - Satis kalemi

### 2.2 Enum'lar `Domain\Entities\Enums\`
- [x] `OrderStatus.cs` -> `Pending`, `Confirmed`, `Shipped`, `Delivered`, `Cancelled`

### 2.3 Exception Siniflari `Domain\Exceptions\`
- [ ] `NotFoundException.cs` - Genel bulunamadi exception'i
- [ ] `DrugNotFoundException.cs`
- [ ] `InsufficientStockException.cs`
- [ ] `CustomerNotFoundException.cs`

---

## ASAMA 3 - Application Katmani

### 3.1 Ortak (Common) Tipler `Application\Common\`
- [x] `Common\Requests\PageRequest.cs` - Tum feature'lar icin ortak sayfalama istegi
- [x] `Common\Responses\GetListResponse.cs` - Tum sayfali liste yanitlari icin generic model

### 3.2 Drug Feature `Application\Features\Drugs\`
- [x] `Commands\Create\CreateDrugCommand.cs` + Handler + Response *(mevcut)*
- [x] `Commands\Update\UpdateDrugCommand.cs` + Handler + Response *(mevcut)*
- [x] `Commands\Delete\DeleteDrugCommand.cs` + Handler + Response *(mevcut)*
- [x] `Queries\GetList\GetListDrugQuery.cs` + Handler + DTO *(mevcut)*
- [x] `Queries\GetById\GetByIdDrugQuery.cs` + Handler + Response *(mevcut)*
- [x] `Profiles\MappingProfiles.cs` *(mevcut)*
- [ ] `Commands\Create\CreateDrugCommandValidator.cs`
- [ ] `Commands\Update\UpdateDrugCommandValidator.cs`

### 3.3 Customer Feature `Application\Features\Customers\`
- [x] `Commands\Create\CreateCustomerCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateCustomerCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteCustomerCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListCustomerQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdCustomerQuery.cs` + Handler + Response
- [x] `Profiles\CustomerMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.4 Supplier Feature `Application\Features\Suppliers\`
- [x] `Commands\Create\CreateSupplierCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateSupplierCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteSupplierCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListSupplierQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdSupplierQuery.cs` + Handler + Response
- [x] `Profiles\SupplierMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.5 Warehouse Feature `Application\Features\Warehouses\`
- [x] `Commands\Create\CreateWarehouseCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateWarehouseCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteWarehouseCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListWarehouseQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdWarehouseQuery.cs` + Handler + Response
- [x] `Profiles\WarehouseMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.6 Stock Feature `Application\Features\Stocks\`
- [x] `Commands\Create\CreateStockCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateStockCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteStockCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListStockQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdStockQuery.cs` + Handler + Response
- [x] `Profiles\StockMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.7 Order Feature `Application\Features\Orders\`
- [x] `Commands\Create\CreateOrderCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateOrderCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteOrderCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListOrderQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdOrderQuery.cs` + Handler + Response
- [x] `Profiles\OrderMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.8 Sale Feature `Application\Features\Sales\`
- [x] `Commands\Create\CreateSaleCommand.cs` + Handler + Response
- [x] `Commands\Update\UpdateSaleCommand.cs` + Handler + Response
- [x] `Commands\Delete\DeleteSaleCommand.cs` + Handler + Response
- [x] `Queries\GetList\GetListSaleQuery.cs` + Handler + DTO
- [x] `Queries\GetById\GetByIdSaleQuery.cs` + Handler + Response
- [x] `Profiles\SaleMappingProfiles.cs`
- [ ] Validator siniflari (FluentValidation)

### 3.9 Servis Kaydi
- [x] `ApplicationServiceRegistration.cs` *(MediatR + AutoMapper kayitli)*
- [ ] FluentValidation DI kaydini ekle

---

## ASAMA 4 - Persistence Katmani

### 4.1 DbContext `Persistence\Contexts\`
- [x] `BaseDbContext.cs` - Tum entity'ler icin DbSet<T> tanimlandi

### 4.2 EF Core Konfigurasyonlari `Persistence\EntityConfigurations\`
- [x] `DrugConfiguration.cs`
- [x] `SupplierConfiguration.cs`
- [x] `WarehouseConfiguration.cs`
- [x] `StockConfiguration.cs`
- [x] `CustomerConfiguration.cs`
- [x] `OrderConfiguration.cs`
- [x] `OrderItemConfiguration.cs`
- [x] `SaleConfiguration.cs`
- [x] `SaleItemConfiguration.cs`

### 4.3 Repository Arayuzleri `Persistence\Repositories\{Entity}\`
- [x] `IDrugRepository.cs`
- [x] `ICustomerRepository.cs`
- [x] `ISupplierRepository.cs`
- [x] `IWarehouseRepository.cs`
- [x] `IStockRepository.cs`
- [x] `IOrderRepository.cs`
- [x] `IOrderItemRepository.cs`
- [x] `ISaleRepository.cs`
- [x] `ISaleItemRepository.cs`

### 4.4 Repository Implementasyonlari `Persistence\Repositories\{Entity}\`
- [x] `DrugRepository.cs`
- [x] `CustomerRepository.cs`
- [x] `SupplierRepository.cs`
- [x] `WarehouseRepository.cs`
- [x] `StockRepository.cs`
- [x] `OrderRepository.cs`
- [x] `OrderItemRepository.cs`
- [x] `SaleRepository.cs`
- [x] `SaleItemRepository.cs`

### 4.5 Servis Kaydi
- [x] `PersistenceServiceRegistration.cs` - DbContext + tum Repository DI kayitlari mevcut

### 4.6 Migration
- [ ] Gercek DB provider'a gec (su an InMemoryDatabase kullaniliyor)
- [ ] Ilk migration olustur: `dotnet ef migrations add InitialCreate --project Persistence --startup-project WebAPI`
- [ ] Veritabanini guncelle: `dotnet ef database update --project Persistence --startup-project WebAPI`

---

## ASAMA 5 - WebAPI Katmani

### 5.1 Controller'lar `WebAPI\Controllers\`
- [x] `BaseController.cs` *(mevcut - Mediator property barindiruyor)*
- [x] `DrugsController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `CustomersController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `SuppliersController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `WarehousesController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `StocksController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `OrdersController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [x] `SalesController.cs` - GET, GET/{id}, POST, PUT, DELETE
- [ ] `OrderItemsController.cs` - OrderItem yonetimi icin
- [ ] `SaleItemsController.cs` - SaleItem yonetimi icin

### 5.2 Middleware `WebAPI\Middlewares\`
- [ ] `ExceptionHandlingMiddleware.cs` - Global hata yakalama ve anlamli HTTP cevabi dondurme

### 5.3 Program.cs Guncellemeleri
- [x] `ApplicationServiceRegistration` ekli *(mevcut)*
- [x] `PersistenceServiceRegistration` ekli *(mevcut)*
- [x] Swagger/OpenAPI konfigurasyonu ekli *(mevcut)*
- [ ] `ExceptionHandlingMiddleware` ekle
- [ ] `appsettings.json` icine connection string ekle

---

## ASAMA 6 - Ek Gelistirmeler (Opsiyonel)

- [ ] `FluentValidation` - Her Command icin Validator sinifi
- [ ] `Domain\Exceptions\` - Ozel exception siniflari (NotFoundException, InsufficientStockException vb.)
- [ ] Global `ExceptionHandlingMiddleware` - Domain exception'larini HTTP 404/422 olarak donustur
- [ ] `Serilog` entegrasyonu - Loglama
- [ ] `Result<T>` pattern - Tutarli hata yonetimi
- [ ] OrderItem ve SaleItem icin tam CQRS feature katmani
- [ ] Unit Testler - Application ve Domain katmanlari icin (xUnit + FluentAssertions + Moq)
- [ ] Integration Testler - WebAPI endpoint'leri icin
- [ ] Production ortami icin InMemory DB yerine SQL Server / PostgreSQL kullan
- [ ] `CancellationToken` destegini DrugsController'a da ekle

---

## Proje Katman Bagimliliklari (Referans)

```
Domain         <- hicbir projeye bagimli degil
Application    <- Domain
Persistence    <- Application + Domain
Infrastructure <- Application
WebAPI         <- Application + Persistence + Infrastructure
```

---

## Copilot Tarafindan Yapilan Degisiklikler

### Repository Tutarsizliklari Giderildi
- `SaleItemRepository`, `StockRepository`, `SupplierRepository`, `WarehouseRepository` dosyalarindaki namespace cakismalari duzeltildi (fully qualified type names kullanildi).
- `IStockRepository` ve `ISupplierRepository` arayuzlerine XML doc comment eklendi.
- `SupplierRepository`'deki gereksiz `using Persistence.Repositories.Drug;` kaldirildi.

### Ortak (Common) Tipler Eklendi
- `Application\Common\Requests\PageRequest.cs` - Tum feature'lar icin ortak sayfalama istegi.
- `Application\Common\Responses\GetListResponse.cs` - Tum sayfali liste yanitlari icin generic model.
- Mevcut `Drug` feature dosyalari bu ortak tipleri kullanacak sekilde guncellendi.

### Eksik DbSet Eklendi
- `Persistence\Contexts\BaseDbContext.cs` - `DbSet<Customer> Customers` eksikti, eklendi.

### 6 Yeni Feature - CQRS Katmani Olusturuldu
Her biri icin Commands (Create/Update/Delete) + Queries (GetList/GetById) + MappingProfile:
- Customer Feature (16 dosya)
- Supplier Feature (16 dosya)
- Warehouse Feature (16 dosya)
- Stock Feature (16 dosya)
- Order Feature (16 dosya)
- Sale Feature (16 dosya)

### 6 Yeni WebAPI Controller Eklendi
- `CustomersController` - GET /api/customers, GET /api/customers/{id}, POST, PUT, DELETE
- `SuppliersController` - GET /api/suppliers, GET /api/suppliers/{id}, POST, PUT, DELETE
- `WarehousesController` - GET /api/warehouses, GET /api/warehouses/{id}, POST, PUT, DELETE
- `StocksController` - GET /api/stocks, GET /api/stocks/{id}, POST, PUT, DELETE
- `OrdersController` - GET /api/orders, GET /api/orders/{id}, POST, PUT, DELETE
- `SalesController` - GET /api/sales, GET /api/sales/{id}, POST, PUT, DELETE