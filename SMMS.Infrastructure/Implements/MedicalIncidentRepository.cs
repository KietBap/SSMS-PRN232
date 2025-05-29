using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class MedicalIncidentRepository : RepositoryBase<MedicalIncident>, IMedicalIncidentRepository
    {
        public MedicalIncidentRepository(DatabaseContext context) : base(context) { }
    }
}
