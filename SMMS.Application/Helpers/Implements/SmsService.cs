using SMMS.Application.Helpers.Interface;

namespace SMMS.Application.Helpers.Implements
{
	public class SmsService : ISmsService
	{
		public Task SendSmsAsync(string phoneNumber, string message)
		{
			Console.WriteLine($"Sending SMS to {phoneNumber}: {message}");
			return Task.CompletedTask;
		}
	}
}
