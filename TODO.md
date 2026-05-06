# PharmacyDepot — Yapılacaklar Listesi

## Durum Göstergesi
- [ ] Yapılmadı
- [x] Tamamlandı

---

## AŞAMA 1 — NuGet Paketleri

### Application Projesi
- [x] `MediatR` paketini ekle *(14.1.0)*
- [ ] `FluentValidation` paketini ekle
- [ ] `AutoMapper` paketini ekle
- [ ] `FluentValidation.DependencyInjectionExtensions` paketini ekle

### Persistence Projesi
- [x] `Microsoft.EntityFrameworkCore` paketini ekle
- [x] `Microsoft.EntityFrameworkCore.SqlServer` paketini ekle *(veya Npgsql.EntityFrameworkCore.PostgreSQL)*
- [x] `Microsoft.EntityFrameworkCore.Tools` paketini ekle
- [x] `Microsoft.EntityFrameworkCore.Design` paketini ekle

### WebAPI Projesi
- [x] `Microsoft.EntityFrameworkCore.Design` paketini ekle *(migration için)*

---

## AŞAMA 2 — Domain Katmanı

### 2.1 Entity'ler `Domain\Entities\`
- [x] `Base\BaseEntity.cs` *(mevcut)*
- [x] `Interfaces\IEntityTimeStamps.cs` *(mevcut)*
- [x] `Drug.cs` *(mevcut)*
- [x] `Supplier.cs` — Tedarikçi firması *(mevcut)*
- [x] `Warehouse.cs` — Depo / raf bilgisi
  - `Id (Guid)`, `Name`, `Location`, `Capacity`
- [x] `Stock.cs` — İlaç stok hareketi
  - `Id (Guid)`, `DrugId (FK)`, `WarehouseId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Drug`, `Warehouse`
- [x] `Customer.cs` — Müşteri eczane
  - `Id (Guid)`, `Name`, `LicenseNumber`, `Phone`, `Email`, `Address`
- [x] `Order.cs` — Satın alma siparişi
  - `Id (Guid)`, `SupplierId (FK)`, `OrderDate`, `Status (enum)`
  - Navigation: `Supplier`, `OrderItems`
- [x] `OrderItem.cs` — Sipariş kalemi
  - `Id (Guid)`, `OrderId (FK)`, `DrugId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Order`, `Drug`
- [x] `Sale.cs` — Satış kaydı
  - `Id (Guid)`, `CustomerId (FK)`, `SaleDate`, `TotalAmount`
  - Navigation: `Customer`, `SaleItems`
- [x] `SaleItem.cs` — Satış kalemi
  - `Id (Guid)`, `SaleId (FK)`, `DrugId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Sale`, `Drug`

### 2.2 Enum'lar `Domain\Enums\`
- [x] `OrderStatus.cs` → `Pending`, `Confirmed`, `Shipped`, `Delivered`, `Cancelled`

### 2.3 Exception Sınıfları `Domain\Exceptions\`
- [ ] `NotFoundException.cs` — Genel bulunamadı exception'ı
- [ ] `DrugNotFoundException.cs`
- [ ] `InsufficientStockException.cs`
- [ ] `CustomerNotFoundException.cs`

---

## AŞAMA 3 — Application Katmanı

> **Not:** Repository arayüzleri `Application\Services\Repositories\` altında tutulmaktadır.
> Persistence katmanında `IAsyncRepository<T>` ve `EntityFrameworkRepositoryBase<T>` generic altyapısı zaten mevcuttur.

### 3.1 Repository Arayüzleri `Application\Services\Repositories\`
- [x] `IDrugRepository.cs` *(mevcut)*
- [ ] `ISupplierRepository.cs`
- [ ] `IWarehouseRepository.cs`
- [ ] `IStockRepository.cs`
- [ ] `ICustomerRepository.cs`
- [ ] `IOrderRepository.cs`
- [ ] `ISaleRepository.cs`

### 3.2 Drug Feature'ları `Application\Features\Drugs\`

**Commands**
- [x] `Commands\Create\CreateDrugCommand.cs` + `CreateDrugCommandHandler.cs` *(mevcut)*
- [x] `Commands\Create\CreatedDrugResponse.cs` *(mevcut)*
- [ ] `Commands\Create\CreateDrugCommandValidator.cs`
- [ ] `Commands\Update\UpdateDrugCommand.cs` + `UpdateDrugCommandHandler.cs`
- [ ] `Commands\Update\UpdateDrugCommandValidator.cs`
- [ ] `Commands\Delete\DeleteDrugCommand.cs` + `DeleteDrugCommandHandler.cs`

**Queries**
- [ ] `Queries\GetAllDrugs\GetAllDrugsQuery.cs` + `GetAllDrugsQueryHandler.cs`
- [ ] `Queries\GetDrugById\GetDrugByIdQuery.cs` + `GetDrugByIdQueryHandler.cs`

**Profiller / Kurallar**
- [ ] `Profiles\DrugMappingProfile.cs` *(AutoMapper)* — `Application\Features\Drugs\Profiles\`
- [ ] `Rules\DrugBusinessRules.cs` — `Application\Features\Drugs\Rules\`

### 3.3 Supplier Feature'ları `Application\Features\Suppliers\`
- [ ] `Commands\Create\CreateSupplierCommand.cs` + Handler + Validator
- [ ] `Queries\GetAll\GetAllSuppliersQuery.cs` + Handler

### 3.4 Stock Feature'ları `Application\Features\Stocks\`
- [ ] `Commands\Add\AddStockCommand.cs` + Handler + Validator
- [ ] `Queries\GetByDrug\GetStockByDrugQuery.cs` + Handler

### 3.5 Order Feature'ları `Application\Features\Orders\`
- [ ] `Commands\Create\CreateOrderCommand.cs` + Handler + Validator
- [ ] `Commands\UpdateStatus\UpdateOrderStatusCommand.cs` + Handler
- [ ] `Queries\GetAll\GetAllOrdersQuery.cs` + Handler
- [ ] `Queries\GetById\GetOrderByIdQuery.cs` + Handler

### 3.6 Sale Feature'ları `Application\Features\Sales\`
- [ ] `Commands\Create\CreateSaleCommand.cs` + Handler + Validator
- [ ] `Queries\GetAll\GetAllSalesQuery.cs` + Handler
- [ ] `Queries\GetById\GetSaleByIdQuery.cs` + Handler

### 3.7 Customer Feature'ları `Application\Features\Customers\`
- [ ] `Commands\Create\CreateCustomerCommand.cs` + Handler + Validator
- [ ] `Queries\GetAll\GetAllCustomersQuery.cs` + Handler

### 3.8 Servis Kaydı
- [x] `ApplicationServiceRegistration.cs` *(mevcut — yalnızca MediatR kayıtlı)*
- [ ] `ApplicationServiceRegistration.cs` içine FluentValidation DI kaydını ekle
- [ ] `ApplicationServiceRegistration.cs` içine AutoMapper DI kaydını ekle

---

## AŞAMA 4 — Persistence Katmanı

> **Not:** `IAsyncRepository<T>`, `ISqlQuery`, `EntityFrameworkRepositoryBase<T>`, Paging ve Dynamic Query altyapısı zaten mevcuttur.

### 4.1 DbContext `Persistence\Contexts\`
- [ ] `AppDbContext.cs`
  - Tüm entity'ler için `DbSet<T>` tanımla
  - `OnModelCreating` override et ve konfigürasyonları uygula

### 4.2 EF Core Konfigürasyonları `Persistence\Configurations\`
- [ ] `DrugConfiguration.cs`
- [ ] `SupplierConfiguration.cs`
- [ ] `WarehouseConfiguration.cs`
- [ ] `StockConfiguration.cs`
- [ ] `CustomerConfiguration.cs`
- [ ] `OrderConfiguration.cs`
- [ ] `OrderItemConfiguration.cs`
- [ ] `SaleConfiguration.cs`
- [ ] `SaleItemConfiguration.cs`

### 4.3 Repository Implementasyonları `Persistence\Repositories\`
- [ ] `DrugRepository.cs`
- [ ] `SupplierRepository.cs`
- [ ] `WarehouseRepository.cs`
- [ ] `StockRepository.cs`
- [ ] `CustomerRepository.cs`
- [ ] `OrderRepository.cs`
- [ ] `SaleRepository.cs`

### 4.4 Servis Kaydı
- [ ] `PersistenceServiceRegistration.cs` — DbContext + Repository DI kaydı

### 4.5 Migration
- [ ] İlk migration oluştur: `dotnet ef migrations add InitialCreate --project Persistence --startup-project WebAPI`
- [ ] Veritabanını güncelle: `dotnet ef database update --project Persistence --startup-project WebAPI`

---

## AŞAMA 5 — WebAPI Katmanı

### 5.1 Controller'lar `WebAPI\Controllers\`
- [x] `BaseController.cs` *(mevcut — Mediator property barındırıyor)*
- [x] `DrugsController.cs` *(mevcut — yalnızca POST endpoint'i var)*
- [ ] `DrugsController.cs` eksik endpoint'leri ekle
  - `GET    /api/drugs`
  - `GET    /api/drugs/{id}`
  - `PUT    /api/drugs/{id}`
  - `DELETE /api/drugs/{id}`
- [ ] `SuppliersController.cs`
  - `GET    /api/suppliers`
  - `POST   /api/suppliers`
- [ ] `WarehousesController.cs`
  - `GET    /api/warehouses`
  - `POST   /api/warehouses`
- [ ] `StocksController.cs`
  - `GET    /api/stocks/drug/{drugId}`
  - `POST   /api/stocks`
- [ ] `OrdersController.cs`
  - `GET    /api/orders`
  - `GET    /api/orders/{id}`
  - `POST   /api/orders`
  - `PATCH  /api/orders/{id}/status`
- [ ] `SalesController.cs`
  - `GET    /api/sales`
  - `GET    /api/sales/{id}`
  - `POST   /api/sales`
- [ ] `CustomersController.cs`
  - `GET    /api/customers`
  - `POST   /api/customers`

### 5.2 Middleware `WebAPI\Middlewares\`
- [ ] `ExceptionHandlingMiddleware.cs` — Global hata yakalama ve anlamlı HTTP cevabı döndürme

### 5.3 Program.cs Güncellemeleri
- [x] `ApplicationServiceRegistration` ekli *(mevcut)*
- [x] Swagger/OpenAPI konfigürasyonu ekli *(mevcut)*
- [ ] `PersistenceServiceRegistration` ekle
- [ ] `ExceptionHandlingMiddleware` ekle
- [ ] `appsettings.json` içine connection string ekle

---

## AŞAMA 6 — Ek İyileştirmeler *(İsteğe Bağlı)*

- [ ] `Serilog` entegrasyonu — Loglama
- [ ] `Result<T>` pattern — Tutarlı hata yönetimi
- [ ] Sayfalama (Pagination) — `GetAll` sorgularına `PageNumber` + `PageSize` ekle *(Persistence paging altyapısı hazır)*
- [ ] Soft Delete — `IEntityTimeStamps` arayüzü mevcut; `DeletedDate` dolu olanları filtrele
- [ ] Unit Testler — `Application` ve `Domain` katmanları için
- [ ] Integration Testler — `WebAPI` endpoint'leri için

---

## Proje Katman Bağımlılıkları (Referans)

```
Domain         ← hiçbir projeye bağımlı değil
Application    ← Domain + Persistence (IAsyncRepository için)
Persistence    ← Application + Domain
Infrastructure ← Application
WebAPI         ← Application + Persistence + Infrastructure
```
