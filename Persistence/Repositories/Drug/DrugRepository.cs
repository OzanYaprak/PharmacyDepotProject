namespace Persistence.Repositories.Drug;

using Domain.Entities;
using Persistence.Contexts;

public class DrugRepository : EntityFrameworkRepositoryBase<Drug, Guid, BaseDbContext>, IDrugRepository
{
    public DrugRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
