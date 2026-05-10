using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories.Customer;
using Persistence.Repositories.Drug;
using Persistence.Repositories.Order;
using Persistence.Repositories.OrderItem;
using Persistence.Repositories.Sale;
using Persistence.Repositories.SaleItem;
using Persistence.Repositories.Stock;
using Persistence.Repositories.Supplier;
using Persistence.Repositories.Warehouse;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContext<BaseDbContext>(options => options.UseInMemoryDatabase("PharmacyDepotInMemoryDb")); // In-memory database for testing
        services.AddDbContext<BaseDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // SQL Server database

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDrugRepository, DrugRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleItemRepository, SaleItemRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

        return services;
    }
}
