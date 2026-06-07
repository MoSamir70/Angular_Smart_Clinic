using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string TypeText => Type.ToString();
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead => Status == NotificationStatus.Read;
}

public class SendNotificationDto
{
    public int PatientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
}

public class NotificationSummaryDto
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public List<NotificationDto> RecentNotifications { get; set; } = new();
}