using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public int? QueueTicketId { get; set; }

    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;

    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateAppointmentDto
{
    public DateTime ScheduledDateTime { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentDto
{
    public int Id { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int DoctorId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class AppointmentConfirmationDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
    public string ClinicPhone { get; set; } = string.Empty;
}