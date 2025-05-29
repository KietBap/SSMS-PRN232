using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class HealthCheckupRepository : RepositoryBase<HealthCheckupRecord>, IHealthCheckupRepository
    {
        public HealthCheckupRepository(DatabaseContext context) : base(context) { }
    }
}
