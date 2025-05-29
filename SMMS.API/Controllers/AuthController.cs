using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("parent/send-otp")]
		public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
		{
			await _authService.SendOtpAsync(request.PhoneNumber);
			return Ok("OTP sent");
		}

		[HttpPost("parent/verify-otp")]
		public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
		{
			var response = await _authService.VerifyOtpAsync(request.PhoneNumber, request.Otp);
			return Ok(response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var response = await _authService.LoginAsync(request.Email, request.Password);
			return Ok(response);
		}
	}
}
