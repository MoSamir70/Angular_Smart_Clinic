using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly SmartClinicDbContext _context;

    public DoctorRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors.FindAsync(id);
    }

    public async Task<Doctor?> GetByIdWithClinicAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.Clinic)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors.Include(d => d.Clinic).ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Doctors
            .Where(d => d.ClinicId == clinicId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(int clinicId)
    {
        return await _context.Doctors
            .Where(d => d.ClinicId == clinicId && d.IsAvailable)
            .ToListAsync();
    }

    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Doctors.AnyAsync(d => d.Id == id);
    }
}