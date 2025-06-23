using Microsoft.AspNetCore.Authorization;
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

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
			{
				return BadRequest("Email is required.");
			}
			if (string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Password is required.");
			}
			var response = await _authService.LoginAsync(request.Email, request.Password);
			return Ok(response);
		}

        [HttpPost("verify-phonenumber")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneRequest request)
        {
            var checker = await _authService.VerifyPhoneNumberAsync(request);

            return Ok(checker);
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(CreateAccountModelView model)
        {
            var checker = await _authService.CreateAccountOtpAsync(model);

            return Ok(checker);
        }

    }
}
