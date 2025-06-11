using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Helpers.Implements;
using SMMS.Application.Services.Interfaces;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/users")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ImportService _importService;

		public UserController(IUserService userService, ImportService importService)
		{
			_userService = userService;
			_importService = importService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsersAsync();
			return Ok(users);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null) return NotFound();
			return Ok(user);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
		{
			var result = await _userService.CreateUserAsync(request);
			if (!result) return BadRequest("Failed to create user.");
			return Ok("User created successfully.");
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
		{
			var result = await _userService.UpdateUserAsync(id, request);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var result = await _userService.DeleteUserAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpGet("profile")]
		[Authorize]
		public async Task<IActionResult> GetMyProfile()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();

			var profile = await _userService.GetMyProfileAsync(userId);
			return Ok(profile);
		}

		[HttpPut("profile")]
		[Authorize]
		public async Task<IActionResult> UpdateMyProfile([FromForm] UserProfileUpdateRequest request)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();

			var result = await _userService.UpdateMyProfileAsync(userId, request);
			if (!result) return BadRequest("Failed to update profile.");
			return NoContent();
		}

		[HttpPost("import-students")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> ImportStudents(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			using (var stream = file.OpenReadStream())
			{
				await _importService.ImportStudentsFromExcelAsync(stream);
			}

			return Ok("Students and parents imported successfully.");
		}

		[HttpGet("students")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllStudents()
		{
			var students = await _userService.GetAllStudentsAsync();
			return Ok(students);
		}

		[HttpGet("students/{studentId}")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetStudentsById(string studentId)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var students = await _userService.GetStudentByIdAsync(studentId);
			return Ok(students);
		}

		[HttpPost("students")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateStudent([FromForm] StudentRequest request, string parentId)
		{
			var result = await _userService.CreateStudentAsync(parentId, request);
			if (!result) return BadRequest("Failed to create student.");
			return Ok("Student created successfully.");
		}

		[HttpPut("students/{studentId}")]
		[Authorize(Roles = "Parent,Admin")]
		public async Task<IActionResult> UpdateStudent(string studentId, [FromForm] StudentRequest request)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var result = await _userService.UpdateStudentAsync(studentId, userId, request);
			if (!result) return BadRequest("Failed to update student.");
			return NoContent();
		}

		[HttpDelete("students/{studentId}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> DeleteStudent(string studentId)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var result = await _userService.DeleteStudentAsync(studentId, userId);
			if (!result) return BadRequest("Failed to delete student.");
			return NoContent();
		}

		[HttpGet("parents/get-all-parent")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllParents()
		{
			var parents = await _userService.GetAllParentsAsync();
			return Ok(parents);
		}
	}
}
