
namespace SMMS.Application.Services.Interfaces
{
	public interface IOtpService
	{
		Task GenerateAndSendOtpAsync(string phoneNumber);
		Task<bool> VerifyOtpAsync(string phoneNumber, string otp);
	}
}
