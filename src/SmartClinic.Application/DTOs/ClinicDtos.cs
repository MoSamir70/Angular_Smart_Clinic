namespace SmartClinic.Application.DTOs;

public class ClinicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int EstimatedWaitTimePerPatient { get; set; }
    public string WorkingHoursStart { get; set; } = string.Empty;
    public string WorkingHoursEnd { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateClinicDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int EstimatedWaitTimePerPatient { get; set; } = 15;
    public string WorkingHoursStart { get; set; } = "09:00";
    public string WorkingHoursEnd { get; set; } = "18:00";
}

public class UpdateClinicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int EstimatedWaitTimePerPatient { get; set; }
    public string WorkingHoursStart { get; set; } = string.Empty;
    public string WorkingHoursEnd { get; set; } = string.Empty;
}