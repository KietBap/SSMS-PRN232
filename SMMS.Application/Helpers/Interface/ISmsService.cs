namespace SMMS.Application.Helpers.Interface
{
	public interface ISmsService
	{
		Task SendSmsAsync(string phoneNumber, string message);
	}
}
