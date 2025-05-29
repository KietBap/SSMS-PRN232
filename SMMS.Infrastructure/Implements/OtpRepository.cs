using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;

namespace SMMS.Infrastructure.Implements
{
	public class OtpRepository : RepositoryBase<Otp>, IOtpRepository
	{
		public OtpRepository(DatabaseContext context) : base(context) { }
	}
}