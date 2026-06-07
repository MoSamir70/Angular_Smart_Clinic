using Microsoft.Extensions.Logging;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendNotificationAsync(int patientId, string title, string message, NotificationType type, NotificationChannel channel = NotificationChannel.InApp)
    {
        try
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Channel = channel,
                Status = NotificationStatus.Pending,
                PatientId = patientId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Notification sent to patient {PatientId}: {Title}", patientId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task SendAppointmentConfirmationAsync(int patientId, Appointment appointment)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
        var clinic = await _unitOfWork.Clinics.GetByIdAsync(appointment.ClinicId);

        var message = $"Your appointment with Dr. {doctor?.FullName} on {appointment.ScheduledDateTime:g} at {clinic?.Name} has been confirmed.";

        await SendNotificationAsync(patientId, "Appointment Confirmed", message, NotificationType.AppointmentConfirmation);
    }

    public async Task SendQueuePositionUpdateAsync(int patientId, QueueTicket ticket)
    {
        var message = $"Your current position in queue is #{ticket.Position}. Estimated wait: {ticket.EstimatedWaitMinutes} minutes.";

        await SendNotificationAsync(patientId, "Queue Position Update", message, NotificationType.QueuePositionUpdate);
    }

    public async Task SendYouAreNextAsync(int patientId, QueueTicket ticket)
    {
        var message = "You are next in queue! Please prepare to see the doctor.";

        await SendNotificationAsync(patientId, "You Are Next!", message, NotificationType.YouAreNext);
    }

    public async Task SendDoctorCalledAsync(int patientId, QueueTicket ticket)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(ticket.DoctorId);
        var message = $"Dr. {doctor?.FullName} is calling you now. Please proceed to the consultation room.";

        await SendNotificationAsync(patientId, "Doctor Called You", message, NotificationType.DoctorCalled);
    }

    public async Task SendDelayAlertAsync(int patientId, int newEstimatedMinutes)
    {
        var message = $"There is a delay. Your new estimated wait time is approximately {newEstimatedMinutes} minutes.";

        await SendNotificationAsync(patientId, "Queue Delay Alert", message, NotificationType.DelayAlert);
    }

    public async Task SendAppointmentReminderAsync(int patientId, Appointment appointment)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
        var message = $"Reminder: Your appointment with Dr. {doctor?.FullName} is at {appointment.ScheduledDateTime:g}.";

        await SendNotificationAsync(patientId, "Appointment Reminder", message, NotificationType.AppointmentReminder);
    }

    public async Task SendBulkNotificationsAsync(IEnumerable<int> patientIds, string title, string message, NotificationType type)
    {
        foreach (var patientId in patientIds)
        {
            try
            {
                await SendNotificationAsync(patientId, title, message, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bulk notification to patient {PatientId}", patientId);
            }
        }
    }
}