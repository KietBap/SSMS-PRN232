using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
    public class ClassRepository : RepositoryBase<SchoolClass>, IClassRepository
    {
        public ClassRepository(DatabaseContext context) : base(context) { }
    }
}
