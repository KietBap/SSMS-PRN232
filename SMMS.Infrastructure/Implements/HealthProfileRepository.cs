using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class HealthProfileRepository : RepositoryBase<HealthProfile>, IHealthProfileRepository
    {
        public HealthProfileRepository(DatabaseContext context) : base(context) { }
    }
}
