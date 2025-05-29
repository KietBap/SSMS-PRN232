using SMMS.Domain.Entity;
namespace SMMS.Application.Helpers.Interface
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(User user);
	}
}
