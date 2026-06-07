using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient?> GetByIdWithDetailsAsync(int id);
    Task<Patient?> GetByPhoneAsync(string phone);
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient> AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}