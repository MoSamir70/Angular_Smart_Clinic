using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly SmartClinicDbContext _context;

    public PatientRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<Patient?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Patients
            .Include(p => p.Appointments)
            .ThenInclude(a => a.Doctor)
            .Include(p => p.QueueTickets)
            .ThenInclude(q => q.Doctor)
            .Include(p => p.Notifications)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Patient?> GetByPhoneAsync(string phone)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Phone == phone);
    }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<Patient> AddAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id);
    }
}