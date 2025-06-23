using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.Services.Interfaces;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/notifications")]
	[Authorize]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpGet]
		public async Task<IActionResult> GetMyNotifications()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
			return Ok(notifications);
		}

		[HttpGet("unread")]
		public async Task<IActionResult> GetUnreadNotifications()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var notifications = await _notificationService.GetIsNotReadNotiByUserIdAsync(userId);
			return Ok(notifications);
		}

		[HttpGet("read")]
		public async Task<IActionResult> GetReadNotifications()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			var notifications = await _notificationService.GetIsReadNotiByUserIdAsync(userId);
			return Ok(notifications);
		}

		[HttpPut("{id}/read")]
		public async Task<IActionResult> MarkAsRead(string id)
		{
			await _notificationService.MarkAsReadAsync(id);
			return NoContent();
		}

		[HttpPut("mark-all-read")]
		public async Task<IActionResult> MarkAllAsRead()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			await _notificationService.MarkAllAsReadAsync(userId);
			return NoContent();
		}

		[HttpDelete("delete-read")]
		public async Task<IActionResult> DeleteReadNotifications()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return Unauthorized();
			await _notificationService.DeleteIsReadNotificationsAsync(userId);
			return NoContent();
		}

	}
}
