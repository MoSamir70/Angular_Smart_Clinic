namespace SmartClinic.Domain.Entities;

public enum TicketStatus
{
    Waiting,
    Called,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public class QueueTicket : BaseEntity
{
    public int TicketNumber { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Waiting;
    public int Position { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CalledTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? EstimatedStartTime { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public int? CalledByDoctorId { get; set; }
    public bool IsVip { get; set; } = false;
    public string? Notes { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    public Appointment? Appointment { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public void Call()
    {
        Status = TicketStatus.Called;
        CalledTime = DateTime.UtcNow;
    }

    public void MarkInProgress()
    {
        Status = TicketStatus.InProgress;
        StartTime = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = TicketStatus.Completed;
        EndTime = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = TicketStatus.Cancelled;
    }

    public void MarkNoShow()
    {
        Status = TicketStatus.NoShow;
    }

    public bool IsWaiting => Status == TicketStatus.Waiting || Status == TicketStatus.Called;
}