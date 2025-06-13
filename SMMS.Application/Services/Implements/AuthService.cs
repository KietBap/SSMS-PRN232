using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Helpers.Implements;
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
        private readonly CloudinaryService _cloudinaryService;

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
			//if (user.Role.RoleName == "Parent")
			//{
			//	Console.WriteLine("Parent Cannot Login With Email Or Password");
			//	throw new Exception("Parent Cannot Login With Email Or Password");
			//}
			var passwordVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
			Console.WriteLine($"Password verified: {passwordVerified}");
			if (!passwordVerified)
			{
				throw new Exception("Invalid credentials");
			}

			var token = _jwtTokenGenerator.GenerateToken(user);
			return new AuthResponse { Token = token, UserId = user.Id };
		}

        private async Task<string?> VerifyFirebasePhoneTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                if (decodedToken.Claims.TryGetValue("phone_number", out var phoneObj))
                    return phoneObj?.ToString();

                return null;
            }
            catch
            {
                return null;
            }
        }


        public async Task<string> VerifyPhoneNumberAsync(VerifyPhoneRequest request)
        {
            try
            {
                var verifiedPhone = await VerifyFirebasePhoneTokenAsync(request.IdToken);
                var normalizedPhone = string.Concat("+84", request.PhoneNumber.AsSpan(1));
                if (verifiedPhone == null || verifiedPhone != normalizedPhone)
                {
                    throw new Exception("Xác thực OTP không hợp lệ hoặc không khớp số điện thoại.");
                }

                var existingPhonenumber = _repositoryManager.UserRepository
                    .FindByCondition(u => u.Phone == request.PhoneNumber, false)
                    .FirstOrDefault();

                if (existingPhonenumber != null)
                {
                    throw new Exception("PhoneNumber already in use");
                }

                return "Ready to create an account !";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> CreateAccountOtpAsync(CreateAccountModelView model)
        {
            try
            {
                // xac thuc id token lan 2
                var verifiedPhone = await VerifyFirebasePhoneTokenAsync(model.IdToken);
                var normalizedPhone = string.Concat("+84", model.PhoneNumber.AsSpan(1));
                if (verifiedPhone == null || verifiedPhone != normalizedPhone)
                {
                    throw new Exception("Xác thực OTP không hợp lệ hoặc không khớp số điện thoại.");
                }

                // kiem tra email da co hay chua
                var existingEmail = _repositoryManager.UserRepository
                    .FindByCondition(u => u.Email == model.Email, false)
                    .FirstOrDefault();

                if (existingEmail != null)
                {
                    throw new Exception("Email already in use");
                }

                // Tạo user mới
                var role = _repositoryManager.RoleRepository
                    .FindByCondition(r => r.RoleName == "User", false)
                    .FirstOrDefault();
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = role.Id,
                    Email = model.Email,
                    Phone = model.PhoneNumber,
                    FullName = model.FullName,
                    Image = await _cloudinaryService.UploadImageAsync(model.Image),
                    CreatedTime = DateTime.Now,
                };
                user.CreatedBy = user.Id.ToString();

                _repositoryManager.UserRepository.Create(user);
                await _repositoryManager.SaveAsync();

                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}