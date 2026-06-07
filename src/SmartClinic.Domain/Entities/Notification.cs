namespace SmartClinic.Domain.Entities;

public enum NotificationType
{
    AppointmentConfirmation,
    AppointmentReminder,
    QueuePositionUpdate,
    YouAreNext,
    DoctorCalled,
    DelayAlert,
    CancellationConfirmation,
    RescheduleConfirmation
}

public enum NotificationChannel
{
    InApp,
    Email,
    Sms,
    Push
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Read
}

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Metadata { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? QueueTicketId { get; set; }
    public QueueTicket? QueueTicket { get; set; }
}