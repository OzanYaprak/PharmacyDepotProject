# PharmacyDepot — Entity İlişki Şeması

## Genel Bakış

Bu belge `Domain\Entities\` altındaki tüm entity sınıflarının birbirleriyle olan ilişkilerini,
FK (Foreign Key) bağımlılıklarını ve navigation property'lerini açıklamaktadır.

---

## Entity Listesi

| Entity | Tablo Karşılığı | Açıklama |
|---|---|---|
| `Drug` | Drugs | İlaç bilgisi |
| `Supplier` | Suppliers | Tedarikçi firma |
| `Warehouse` | Warehouses | Depo / raf |
| `Stock` | Stocks | İlaç stok hareketi |
| `Customer` | Customers | Müşteri eczane |
| `Order` | Orders | Tedarikçiye satın alma siparişi |
| `OrderItem` | OrderItems | Sipariş kalemi |
| `Sale` | Sales | Müşteriye satış kaydı |
| `SaleItem` | SaleItems | Satış kalemi |

---

## İlişki Şeması (ASCII)

```
┌─────────────┐        1        N  ┌─────────────┐        N        1  ┌─────────────┐
│   Supplier  │──────────────────▶ │    Order    │◀────────────────── │  OrderItem  │
│─────────────│                    │─────────────│                    │─────────────│
│ Id          │                    │ Id          │                    │ Id          │
│ Name        │                    │ SupplierId ●│                    │ OrderId    ●│
│ ContactPers.│                    │ OrderDate   │                    │ DrugId     ●│
│ Phone       │                    │ Status      │                    │ Quantity    │
│ Email       │                    │─────────────│                    │ UnitPrice   │
│ Address     │                    │ nav:        │                    │─────────────│
│─────────────│                    │  Supplier   │                    │ nav:        │
│ nav:        │                    │  OrderItems │                    │  Order      │
│  Orders     │                    └─────────────┘                    │  Drug       │
└─────────────┘                                                        └──────┬──────┘
                                                                              │
                                                                              │ N
                                                                              ▼
┌─────────────┐        1        N  ┌─────────────┐        N        1  ┌──────┴──────┐
│  Customer   │──────────────────▶ │    Sale     │◀────────────────── │  SaleItem   │
│─────────────│                    │─────────────│                    │─────────────│
│ Id          │                    │ Id          │                    │ Id          │
│ Name        │                    │ CustomerId ●│                    │ SaleId     ●│
│ LicenseNo.  │                    │ SaleDate    │                    │ DrugId     ●│
│ Phone       │                    │ TotalAmount │                    │ Quantity    │
│ Email       │                    │─────────────│                    │ UnitPrice   │
│ Address     │                    │ nav:        │                    │─────────────│
│─────────────│                    │  Customer   │                    │ nav:        │
│ nav:        │                    │  SaleItems  │                    │  Sale       │
│  Sales      │                    └─────────────┘                    │  Drug       │
└─────────────┘                                                        └──────┬──────┘
                                                                              │
                                                                              │ N
                                                                              ▼
┌─────────────┐        1        N  ┌─────────────┐                    ┌──────┴──────┐
│  Warehouse  │──────────────────▶ │    Stock    │──────────────────▶ │    Drug     │
│─────────────│                    │─────────────│        N        1  │─────────────│
│ Id          │                    │ Id          │                    │ Id          │
│ Name        │                    │ WarehouseId●│                    │ Name        │
│ Location    │                    │ DrugId     ●│                    │ GTIN        │
│ Capacity    │                    │ Quantity    │                    │ SN          │
│─────────────│                    │ UnitPrice   │                    │ BN          │
│ nav:        │                    │─────────────│                    │ ExpireDate  │
│  Stocks     │                    │ nav:        │                    │─────────────│
└─────────────┘                    │  Drug       │                    │ nav:        │
                                   │  Warehouse  │                    │  Stocks     │
                                   └─────────────┘                    │  OrderItems │
                                                                       │  SaleItems  │
                                                                       └─────────────┘
```

> **●** simgesi Foreign Key (FK) alanını gösterir.

---

## İlişki Türleri

| İlişki | Tür | Açıklama |
|---|---|---|
| `Supplier` → `Order` | **One-to-Many** | Bir tedarikçinin birden fazla siparişi olabilir |
| `Order` → `OrderItem` | **One-to-Many** | Bir sipariş birden fazla kalem içerebilir |
| `Customer` → `Sale` | **One-to-Many** | Bir müşterinin birden fazla satışı olabilir |
| `Sale` → `SaleItem` | **One-to-Many** | Bir satış birden fazla kalem içerebilir |
| `Warehouse` → `Stock` | **One-to-Many** | Bir depoda birden fazla stok kaydı bulunabilir |
| `Drug` → `Stock` | **One-to-Many** | Bir ilaç birden fazla depoda stoklanabilir |
| `Drug` → `OrderItem` | **One-to-Many** | Bir ilaç birden fazla sipariş kaleminde bulunabilir |
| `Drug` → `SaleItem` | **One-to-Many** | Bir ilaç birden fazla satış kaleminde bulunabilir |

---

## Navigation Property Haritası

### `Drug`
```
Drug
 ├── Stocks      : ICollection<Stock>     (1:N → Stock.DrugId)
 ├── OrderItems  : ICollection<OrderItem> (1:N → OrderItem.DrugId)
 └── SaleItems   : ICollection<SaleItem>  (1:N → SaleItem.DrugId)
```

### `Supplier`
```
Supplier
 └── Orders      : ICollection<Order>     (1:N → Order.SupplierId)
```

### `Warehouse`
```
Warehouse
 └── Stocks      : ICollection<Stock>     (1:N → Stock.WarehouseId)
```

### `Stock`
```
Stock
 ├── Drug        : Drug                   (N:1 → Drug.Id)
 └── Warehouse   : Warehouse              (N:1 → Warehouse.Id)
```

### `Customer`
```
Customer
 └── Sales       : ICollection<Sale>      (1:N → Sale.CustomerId)
```

### `Order`
```
Order
 ├── Supplier    : Supplier               (N:1 → Supplier.Id)
 └── OrderItems  : ICollection<OrderItem> (1:N → OrderItem.OrderId)
```

### `OrderItem`
```
OrderItem
 ├── Order       : Order                  (N:1 → Order.Id)
 └── Drug        : Drug                   (N:1 → Drug.Id)
```

### `Sale`
```
Sale
 ├── Customer    : Customer               (N:1 → Customer.Id)
 └── SaleItems   : ICollection<SaleItem>  (1:N → SaleItem.SaleId)
```

### `SaleItem`
```
SaleItem
 ├── Sale        : Sale                   (N:1 → Sale.Id)
 └── Drug        : Drug                   (N:1 → Drug.Id)
```

---

## Taban Sınıf

Tüm entity'ler `BaseEntity<Guid>` sınıfından türetilmiştir:

```
BaseEntity<TId> (implements IEntityTimeStamps)
 ├── Id          : TId
 ├── CreatedDate : DateTime
 ├── UpdatedDate : DateTime?
 └── DeletedDate : DateTime?
```

---

## Enum

### `OrderStatus` (`Domain\Enums\OrderStatus.cs`)

| Değer | İsim | Açıklama |
|---|---|---|
| `0` | `Pending` | Sipariş oluşturuldu, onay bekliyor |
| `1` | `Confirmed` | Tedarikçi tarafından onaylandı |
| `2` | `Shipped` | Kargoya verildi |
| `3` | `Delivered` | Teslim edildi |
| `4` | `Cancelled` | İptal edildi |
