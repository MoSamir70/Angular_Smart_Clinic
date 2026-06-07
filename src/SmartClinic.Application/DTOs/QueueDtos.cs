using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.DTOs;

public class QueueTicketDto
{
    public int Id { get; set; }
    public int TicketNumber { get; set; }
    public TicketStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public int Position { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CalledTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? EstimatedStartTime { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public int? CalledByDoctorId { get; set; }
    public string? CalledByDoctorName { get; set; }
    public bool IsVip { get; set; }
    public string? Notes { get; set; }

    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;

    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
}

public class CreateQueueTicketDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    public bool IsVip { get; set; } = false;
    public string? Notes { get; set; }
    public int? AppointmentId { get; set; }
}

public class QueueStatusDto
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public int QueueCount { get; set; }
    public int? CurrentTicketId { get; set; }
    public string? CurrentPatientName { get; set; }
    public QueueTicketDto? NextPatient { get; set; }
    public List<QueueTicketDto> WaitingList { get; set; } = new();
    public int AverageWaitMinutes { get; set; }
}

public class PatientQueueStatusDto
{
    public int TicketId { get; set; }
    public int TicketNumber { get; set; }
    public TicketStatus Status { get; set; }
    public int Position { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public DateTime? EstimatedStartTime { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public bool IsMyTurn => Status == TicketStatus.Called;
}

public class QueueUpdateDto
{
    public int ClinicId { get; set; }
    public int DoctorId { get; set; }
    public int QueueCount { get; set; }
    public int? CurrentTicketId { get; set; }
    public List<QueueTicketDto> WaitingPatients { get; set; } = new();
}