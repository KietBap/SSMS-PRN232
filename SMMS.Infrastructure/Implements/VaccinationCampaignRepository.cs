using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class VaccinationCampaignRepository : RepositoryBase<VaccinationCampaign>, IVaccinationCampaignRepository
    {
        public VaccinationCampaignRepository(DatabaseContext context) : base(context) { }
    }
}
