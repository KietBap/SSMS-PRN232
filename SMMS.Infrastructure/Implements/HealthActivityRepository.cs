using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class HealthActivityRepository : RepositoryBase<HealthActivity>, IHealthActivityRepository
    {
        public HealthActivityRepository(DatabaseContext context) : base(context) { }
    }
}
