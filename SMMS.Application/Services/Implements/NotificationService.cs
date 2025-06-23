using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class NotificationService : INotificationService
	{
		private readonly IRepositoryManager _repositoryManager;

		public NotificationService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task CreateNotificationAsync(string userId, string title, string message, string eventId)
		{
			var notification = new Notification
			{
				UserId = userId,
				Title = title,
				Message = message,
				IsRead = false,
				CreatedTime = DateTimeOffset.UtcNow,
				EventId = eventId ?? string.Empty
			};
			_repositoryManager.NotificationRepository.Create(notification);
			await _repositoryManager.SaveAsync();
		}

		public async Task<List<NotificationResponse>> GetNotificationsByUserIdAsync(string userId)
		{
			var notifications = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.UserId == userId && n.DeletedTime == null, false)
				.OrderByDescending(n => n.CreatedTime)
				.Select(n => new NotificationResponse
				{
					Id = n.Id,
					Title = n.Title,
					Message = n.Message,
					IsRead = n.IsRead,
					CreatedTime = n.CreatedTime,
					EventId = n.EventId ?? string.Empty
				})
				.ToListAsync();
			return notifications;
		}
		public async Task<List<NotificationResponse>> GetIsNotReadNotiByUserIdAsync(string userId)
		{
			var notifications = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.UserId == userId && n.DeletedTime == null && !n.IsRead, false)
				.OrderByDescending(n => n.CreatedTime)
				.Select(n => new NotificationResponse
				{
					Id = n.Id,
					Title = n.Title,
					Message = n.Message,
					IsRead = n.IsRead,
					CreatedTime = n.CreatedTime,
					EventId = n.EventId ?? string.Empty
				})
				.ToListAsync();
			return notifications;
		}
		public async Task<List<NotificationResponse>> GetIsReadNotiByUserIdAsync(string userId)
		{
			var notifications = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.UserId == userId && n.DeletedTime == null && n.IsRead, false)
				.OrderByDescending(n => n.CreatedTime)
				.Select(n => new NotificationResponse
				{
					Id = n.Id,
					Title = n.Title,
					Message = n.Message,
					IsRead = n.IsRead,
					CreatedTime = n.CreatedTime,
					EventId = n.EventId ?? string.Empty
				})
				.ToListAsync();
			return notifications;
		}

		public async Task MarkAsReadAsync(string notificationId)
		{
			var notification = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.Id == notificationId && n.DeletedTime == null, true)
				.FirstOrDefaultAsync();
			if (notification != null)
			{
				notification.IsRead = true;
				notification.LastUpdatedTime = DateTimeOffset.UtcNow;
				_repositoryManager.NotificationRepository.Update(notification);
				await _repositoryManager.SaveAsync();
			}
		}
		public async Task MarkAllAsReadAsync(string userId)
		{
			var notifications = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.UserId == userId && !n.IsRead && n.DeletedTime == null, true)
				.ToListAsync();
			foreach (var notification in notifications)
			{
				notification.IsRead = true;
				notification.LastUpdatedTime = DateTimeOffset.UtcNow;
				_repositoryManager.NotificationRepository.Update(notification);
			}
			await _repositoryManager.SaveAsync();
		}
		public async Task DeleteIsReadNotificationsAsync(string userId)
		{
			var unreadNotifications = await _repositoryManager.NotificationRepository
				.FindByCondition(n => n.UserId == userId && n.IsRead && n.DeletedTime == null, true)
				.ToListAsync();

			foreach (var notification in unreadNotifications)
			{
				notification.DeletedTime = DateTimeOffset.UtcNow;
				_repositoryManager.NotificationRepository.Update(notification);
			}

			await _repositoryManager.SaveAsync();
		}
	}
}
