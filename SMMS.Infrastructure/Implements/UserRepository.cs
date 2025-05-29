using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
	public class UserRepository : RepositoryBase<User>, IUserRepository
	{
		public UserRepository(DatabaseContext context) : base(context)
		{

		}
	}
}
