using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class ConselingRepository : RepositoryBase<ConselingSchedule>, IConselingRepository
    {
        public ConselingRepository(DatabaseContext context) : base(context) { }
    }
}
