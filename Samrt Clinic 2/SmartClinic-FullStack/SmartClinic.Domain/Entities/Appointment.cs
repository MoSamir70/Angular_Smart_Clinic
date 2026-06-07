namespace SmartClinic.Domain.Entities;

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public class Appointment : BaseEntity
{
    public DateTime ScheduledDateTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public int? QueueTicketId { get; set; }
    public QueueTicket? QueueTicket { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    public bool CanCancel()
    {
        var hoursUntilAppointment = (ScheduledDateTime - DateTime.UtcNow).TotalHours;
        return hoursUntilAppointment >= 2 && Status == AppointmentStatus.Scheduled;
    }

    public bool CanReschedule()
    {
        var hoursUntilAppointment = (ScheduledDateTime - DateTime.UtcNow).TotalHours;
        return hoursUntilAppointment >= 2 && (Status == AppointmentStatus.Scheduled || Status == AppointmentStatus.Confirmed);
    }

    public void MarkAsConfirmed()
    {
        if (Status == AppointmentStatus.Scheduled)
            Status = AppointmentStatus.Confirmed;
    }

    public void MarkAsCancelled()
    {
        if (Status != AppointmentStatus.Completed)
            Status = AppointmentStatus.Cancelled;
    }

    public void MarkAsNoShow()
    {
        if (ScheduledDateTime < DateTime.UtcNow && Status == AppointmentStatus.Scheduled)
            Status = AppointmentStatus.NoShow;
    }
}