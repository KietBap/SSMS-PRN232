using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class MedicalStockRepository : RepositoryBase<MedicalStock>, IMedicalStockRepository
    {
        public MedicalStockRepository(DatabaseContext context) : base(context) { }
    }
}
