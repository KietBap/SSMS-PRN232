using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class MedicalRequestRepository : RepositoryBase<MedicalRequest>, IMedicalRequestRepository
    {
        public MedicalRequestRepository(DatabaseContext context) : base(context) { }
    }
}
