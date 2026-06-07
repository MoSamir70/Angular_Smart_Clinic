using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id);
    Task<Doctor?> GetByIdWithClinicAsync(int id);
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task<IEnumerable<Doctor>> GetByClinicIdAsync(int clinicId);
    Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(int clinicId);
    Task<Doctor> AddAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}