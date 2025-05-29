using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class MedicalUsageRepository : RepositoryBase<MedicalUsage>, IMedicalUsageRepository
    {
        public MedicalUsageRepository(DatabaseContext context) : base(context) { }
    }
}
