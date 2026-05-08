namespace Persistence.Repositories.Customer;

using Persistence.Contexts;
using Domain.Entities;

public class CustomerRepository : EntityFrameworkRepositoryBase<Customer, Guid, BaseDbContext>, ICustomerRepository
{
    public CustomerRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
