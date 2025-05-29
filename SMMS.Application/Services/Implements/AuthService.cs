
using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Helpers.Interface;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class AuthService : IAuthService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IOtpService _otpService;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;

		public AuthService(IRepositoryManager repositoryManager, IOtpService otpService, IJwtTokenGenerator jwtTokenGenerator)
		{
			_repositoryManager = repositoryManager;
			_otpService = otpService;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		public async Task SendOtpAsync(string phoneNumber)
		{
			await _otpService.GenerateAndSendOtpAsync(phoneNumber);
		}

		public async Task<AuthResponse> VerifyOtpAsync(string phoneNumber, string otp)
		{
			var isValid = await _otpService.VerifyOtpAsync(phoneNumber, otp);
			if (!isValid)
			{
				throw new Exception("Invalid OTP");
			}

			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Phone == phoneNumber, false)
				.Include(u => u.Role)
				.FirstOrDefault();
			if (user == null)
			{
				var role = _repositoryManager.RoleRepository.FindByCondition(r => r.RoleName == "User", false)
					.FirstOrDefault();
				if (role == null)
				{
					throw new Exception("Parent role not found");
				}
				user = new User
				{
					Phone = phoneNumber,
					RoleId = role.Id,
					FullName = "New Parent",
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				};
				_repositoryManager.UserRepository.Create(user);
				await _repositoryManager.SaveAsync();
			}
			else if (user.Role.RoleName != "Parent")
			{
				throw new Exception("OTP login is only for parents");
			}

			var token = _jwtTokenGenerator.GenerateToken(user);
			return new AuthResponse { Token = token, UserId = user.Id };
		}
		public async Task<AuthResponse> LoginAsync(string email, string password)
		{
			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Email == email, false)
				.Include(u => u.Role)
				.FirstOrDefault();
			if (user == null)
			{
				Console.WriteLine($"User not found for email: {email}");
				throw new Exception("Invalid credentials");
			}
			Console.WriteLine($"User found: {user.Id}, Role: {user.Role?.RoleName}");
			if (user.Role.RoleName == "User")
			{
				Console.WriteLine("User role is 'User'");
				throw new Exception("Invalid credentials");
			}
			var passwordVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
			Console.WriteLine($"Password verified: {passwordVerified}");
			if (!passwordVerified)
			{
				throw new Exception("Invalid credentials");
			}

			var token = _jwtTokenGenerator.GenerateToken(user);
			return new AuthResponse { Token = token, UserId = user.Id };
		}
	}
}
