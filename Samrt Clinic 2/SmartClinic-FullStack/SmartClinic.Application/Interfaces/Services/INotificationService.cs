using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendNotificationAsync(int patientId, string title, string message, NotificationType type, NotificationChannel channel = NotificationChannel.InApp);
    Task SendAppointmentConfirmationAsync(int patientId, Appointment appointment);
    Task SendQueuePositionUpdateAsync(int patientId, QueueTicket ticket);
    Task SendYouAreNextAsync(int patientId, QueueTicket ticket);
    Task SendDoctorCalledAsync(int patientId, QueueTicket ticket);
    Task SendDelayAlertAsync(int patientId, int newEstimatedMinutes);
    Task SendAppointmentReminderAsync(int patientId, Appointment appointment);
    Task SendBulkNotificationsAsync(IEnumerable<int> patientIds, string title, string message, NotificationType type);
}