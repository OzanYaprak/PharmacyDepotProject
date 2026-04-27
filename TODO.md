# PharmacyDepot — Yapılacaklar Listesi

## Durum Göstergesi
- [ ] Yapılmadı
- [x] Tamamlandı

---

## AŞAMA 1 — NuGet Paketleri

### Application Projesi
- [x] `MediatR` paketini ekle
- [ ] `FluentValidation` paketini ekle
- [ ] `AutoMapper` paketini ekle
- [ ] `FluentValidation.DependencyInjectionExtensions` paketini ekle

### Persistence Projesi
- [ ] `Microsoft.EntityFrameworkCore` paketini ekle
- [ ] `Microsoft.EntityFrameworkCore.SqlServer` paketini ekle *(veya Npgsql.EntityFrameworkCore.PostgreSQL)*
- [ ] `Microsoft.EntityFrameworkCore.Tools` paketini ekle
- [ ] `Microsoft.EntityFrameworkCore.Design` paketini ekle

### WebAPI Projesi
- [ ] `Microsoft.EntityFrameworkCore.Design` paketini ekle *(migration için)*

---

## AŞAMA 2 — Domain Katmanı

### 2.1 Yeni Entity'ler `Domain\Entities\`
- [x] `BaseEntity.cs` *(mevcut)*
- [x] `Drug.cs` *(mevcut)*
- [x] `Supplier.cs` — Tedarikçi firması
  - `Id (Guid)`, `Name`, `ContactPerson`, `Phone`, `Email`, `Address`
- [ ] `Warehouse.cs` — Depo / raf bilgisi
  - `Id (Guid)`, `Name`, `Location`, `Capacity`
- [ ] `Stock.cs` — İlaç stok hareketi
  - `Id (Guid)`, `DrugId (FK)`, `WarehouseId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Drug`, `Warehouse`
- [ ] `Customer.cs` — Müşteri eczane
  - `Id (Guid)`, `Name`, `LicenseNumber`, `Phone`, `Email`, `Address`
- [ ] `Order.cs` — Satın alma siparişi
  - `Id (Guid)`, `SupplierId (FK)`, `OrderDate`, `Status (enum)`
  - Navigation: `Supplier`, `OrderItems`
- [ ] `OrderItem.cs` — Sipariş kalemi
  - `Id (Guid)`, `OrderId (FK)`, `DrugId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Order`, `Drug`
- [ ] `Sale.cs` — Satış kaydı
  - `Id (Guid)`, `CustomerId (FK)`, `SaleDate`, `TotalAmount`
  - Navigation: `Customer`, `SaleItems`
- [ ] `SaleItem.cs` — Satış kalemi
  - `Id (Guid)`, `SaleId (FK)`, `DrugId (FK)`, `Quantity`, `UnitPrice`
  - Navigation: `Sale`, `Drug`

### 2.2 Enum'lar `Domain\Enums\`
- [ ] `OrderStatus.cs` → `Pending`, `Confirmed`, `Shipped`, `Delivered`, `Cancelled`

### 2.3 Repository Interface'leri `Domain\Interfaces\`
- [ ] `IRepository.cs` — Generic repository arayüzü
  - `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`
- [ ] `IDrugRepository.cs`
- [ ] `ISupplierRepository.cs`
- [ ] `IStockRepository.cs`
- [ ] `ICustomerRepository.cs`
- [ ] `IOrderRepository.cs`
- [ ] `ISaleRepository.cs`

### 2.4 Exception Sınıfları `Domain\Exceptions\`
- [ ] `NotFoundException.cs` — Genel bulunamadı exception'ı
- [ ] `DrugNotFoundException.cs`
- [ ] `InsufficientStockException.cs`
- [ ] `CustomerNotFoundException.cs`

---

## AŞAMA 3 — Application Katmanı

### 3.1 Drug Feature'ları `Application\Features\Drugs\`

**Commands**
- [ ] `CreateDrug\CreateDrugCommand.cs` + `CreateDrugCommandHandler.cs`
- [ ] `CreateDrug\CreateDrugCommandValidator.cs`
- [ ] `UpdateDrug\UpdateDrugCommand.cs` + `UpdateDrugCommandHandler.cs`
- [ ] `UpdateDrug\UpdateDrugCommandValidator.cs`
- [ ] `DeleteDrug\DeleteDrugCommand.cs` + `DeleteDrugCommandHandler.cs`

**Queries**
- [ ] `GetAllDrugs\GetAllDrugsQuery.cs` + `GetAllDrugsQueryHandler.cs`
- [ ] `GetDrugById\GetDrugByIdQuery.cs` + `GetDrugByIdQueryHandler.cs`

**DTOs** `Application\Features\Drugs\Dtos\`
- [ ] `DrugDto.cs`
- [ ] `CreateDrugDto.cs`
- [ ] `UpdateDrugDto.cs`

### 3.2 Stock Feature'ları `Application\Features\Stocks\`
- [ ] `AddStock\AddStockCommand.cs` + Handler + Validator
- [ ] `GetStockByDrug\GetStockByDrugQuery.cs` + Handler
- [ ] `Dtos\StockDto.cs`

### 3.3 Order Feature'ları `Application\Features\Orders\`
- [ ] `CreateOrder\CreateOrderCommand.cs` + Handler + Validator
- [ ] `UpdateOrderStatus\UpdateOrderStatusCommand.cs` + Handler
- [ ] `GetAllOrders\GetAllOrdersQuery.cs` + Handler
- [ ] `GetOrderById\GetOrderByIdQuery.cs` + Handler
- [ ] `Dtos\OrderDto.cs`, `OrderItemDto.cs`

### 3.4 Sale Feature'ları `Application\Features\Sales\`
- [ ] `CreateSale\CreateSaleCommand.cs` + Handler + Validator
- [ ] `GetAllSales\GetAllSalesQuery.cs` + Handler
- [ ] `GetSaleById\GetSaleByIdQuery.cs` + Handler
- [ ] `Dtos\SaleDto.cs`, `SaleItemDto.cs`

### 3.5 Supplier Feature'ları `Application\Features\Suppliers\`
- [ ] `CreateSupplier\CreateSupplierCommand.cs` + Handler + Validator
- [ ] `GetAllSuppliers\GetAllSuppliersQuery.cs` + Handler
- [ ] `Dtos\SupplierDto.cs`

### 3.6 Customer Feature'ları `Application\Features\Customers\`
- [ ] `CreateCustomer\CreateCustomerCommand.cs` + Handler + Validator
- [ ] `GetAllCustomers\GetAllCustomersQuery.cs` + Handler
- [ ] `Dtos\CustomerDto.cs`

### 3.7 Servis Kaydı
- [ ] `Application\ServiceRegistration.cs` — MediatR + FluentValidation + AutoMapper DI kaydı

---

## AŞAMA 4 — Persistence Katmanı

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
- [ ] `Repository.cs` — Generic repository implementasyonu
- [ ] `DrugRepository.cs`
- [ ] `SupplierRepository.cs`
- [ ] `StockRepository.cs`
- [ ] `CustomerRepository.cs`
- [ ] `OrderRepository.cs`
- [ ] `SaleRepository.cs`

### 4.4 Servis Kaydı
- [ ] `Persistence\ServiceRegistration.cs` — DbContext + Repository DI kaydı

### 4.5 Migration
- [ ] İlk migration oluştur: `dotnet ef migrations add InitialCreate --project Persistence --startup-project WebAPI`
- [ ] Veritabanını güncelle: `dotnet ef database update --project Persistence --startup-project WebAPI`

---

## AŞAMA 5 — WebAPI Katmanı

### 5.1 Controller'lar `WebAPI\Controllers\`
- [ ] `DrugsController.cs`
  - `GET    /api/drugs`
  - `GET    /api/drugs/{id}`
  - `POST   /api/drugs`
  - `PUT    /api/drugs/{id}`
  - `DELETE /api/drugs/{id}`
- [ ] `SuppliersController.cs`
  - `GET    /api/suppliers`
  - `POST   /api/suppliers`
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
- [ ] `Application.ServiceRegistration` ekle
- [ ] `Persistence.ServiceRegistration` ekle
- [ ] `ExceptionHandlingMiddleware` ekle
- [ ] Swagger/OpenAPI konfigürasyonu kontrol et

---

## AŞAMA 6 — Ek İyileştirmeler *(İsteğe Bağlı)*

- [ ] `Serilog` entegrasyonu — Loglama
- [ ] `Result<T>` pattern — Tutarlı hata yönetimi
- [ ] Sayfalama (Pagination) — `GetAll` sorgularına `PageNumber` + `PageSize` ekle
- [ ] Soft Delete — `DeletedDate` dolu olanları sorgulardan filtrele
- [ ] Unit Testler — `Application` ve `Domain` katmanları için
- [ ] Integration Testler — `WebAPI` endpoint'leri için
- [ ] `appsettings.json` — Connection string yapılandırması

---

## Proje Katman Bağımlılıkları (Referans)

```
Domain        ← hiçbir projeye bağımlı değil
Application   ← Domain
Persistence   ← Application + Domain
Infrastructure← Application
WebAPI        ← Application + Persistence + Infrastructure
```
