using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<Appointment?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByClinicIdAsync(int clinicId);
    Task<IEnumerable<Appointment>> GetByDateAsync(int doctorId, DateTime date);
    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int clinicId);
    Task<Appointment> AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime dateTime, int? excludeId = null);
}