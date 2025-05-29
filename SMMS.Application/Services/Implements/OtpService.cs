using SMMS.Application.Helpers.Interface;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class OtpService : IOtpService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly ISmsService _smsService;

		public OtpService(IRepositoryManager repositoryManager, ISmsService smsService)
		{
			_repositoryManager = repositoryManager;
			_smsService = smsService;
		}

		public async Task GenerateAndSendOtpAsync(string phoneNumber)
		{
			var otpCode = new Random().Next(100000, 999999).ToString();
			var expirationTime = DateTimeOffset.UtcNow.AddMinutes(5);

			var existingOtps = _repositoryManager.OtpRepository.FindByCondition(o => o.PhoneNumber == phoneNumber && !o.IsUsed && o.ExpirationTime > DateTimeOffset.UtcNow, false).ToList();
			foreach (var existingOtp in existingOtps)
			{
				existingOtp.IsUsed = true;
				_repositoryManager.OtpRepository.Update(existingOtp);
			}

			var otp = new Otp
			{
				PhoneNumber = phoneNumber,
				OtpCode = otpCode,
				ExpirationTime = expirationTime,
				IsUsed = false,
				UserId = null
			};
			_repositoryManager.OtpRepository.Create(otp);
			await _repositoryManager.SaveAsync();

			await _smsService.SendSmsAsync(phoneNumber, $"Your OTP is {otpCode}");
		}

		public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp)
		{
			var currentTime = DateTimeOffset.UtcNow;
			var otpEntity = _repositoryManager.OtpRepository.FindByCondition(o => o.PhoneNumber == phoneNumber && o.OtpCode == otp && !o.IsUsed && o.ExpirationTime > currentTime, false)
				.FirstOrDefault();
			if (otpEntity != null)
			{
				otpEntity.IsUsed = true;
				_repositoryManager.OtpRepository.Update(otpEntity);
				await _repositoryManager.SaveAsync();
				return true;
			}
			return false;
		}
	}
}
