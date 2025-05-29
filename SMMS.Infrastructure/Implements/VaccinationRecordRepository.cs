using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class VaccinationRecordRepository : RepositoryBase<VaccinationRecord>, IVaccinationRecordRepository
    {
        public VaccinationRecordRepository(DatabaseContext context) : base(context) { }
    }
}
