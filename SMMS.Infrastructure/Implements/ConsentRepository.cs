using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class ConsentRepository : RepositoryBase<ActivityConsent>, IConsentRepository
    {
        public ConsentRepository(DatabaseContext context) : base(context) { }
    }
}
