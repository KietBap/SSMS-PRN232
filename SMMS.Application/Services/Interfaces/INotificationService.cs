
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface INotificationService
	{
		Task CreateNotificationAsync(string userId, string title, string message, string eventId);
		Task<List<NotificationResponse>> GetNotificationsByUserIdAsync(string userId);
		Task<List<NotificationResponse>> GetIsNotReadNotiByUserIdAsync(string userId);
		Task<List<NotificationResponse>> GetIsReadNotiByUserIdAsync(string userId);
		Task MarkAsReadAsync(string notificationId);
		Task MarkAllAsReadAsync(string userId);
		Task DeleteIsReadNotificationsAsync(string userId);
	}
}
