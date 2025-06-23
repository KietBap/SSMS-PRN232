using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class MedicationRequestAdministrationRepository : RepositoryBase<MedicationRequestAdministration>, IMedicationRequestAdministrationRepository
    {
        public MedicationRequestAdministrationRepository(DatabaseContext context) : base(context) { }
    }
}
