using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface IClinicRepository
{
    Task<Clinic?> GetByIdAsync(int id);
    Task<Clinic?> GetByIdWithDoctorsAsync(int id);
    Task<IEnumerable<Clinic>> GetAllAsync();
    Task<Clinic> AddAsync(Clinic clinic);
    Task UpdateAsync(Clinic clinic);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}