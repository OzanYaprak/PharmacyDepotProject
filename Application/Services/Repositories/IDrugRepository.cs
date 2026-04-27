using Domain.Entities;
using Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IDrugRepository : IAsyncRepository<Drug,Guid>
{
}
