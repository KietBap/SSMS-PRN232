

using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponse> LoginAsync(string email, string password);
        Task<string> VerifyPhoneNumberAsync(VerifyPhoneRequest request);
        Task<string> CreateAccountOtpAsync(CreateAccountModelView model);
    }
}
