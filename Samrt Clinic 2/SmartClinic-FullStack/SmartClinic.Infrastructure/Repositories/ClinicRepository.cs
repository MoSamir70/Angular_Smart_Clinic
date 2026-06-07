using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class ClinicRepository : IClinicRepository
{
    private readonly SmartClinicDbContext _context;

    public ClinicRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Clinic?> GetByIdAsync(int id)
    {
        return await _context.Clinics.FindAsync(id);
    }

    public async Task<Clinic?> GetByIdWithDoctorsAsync(int id)
    {
        return await _context.Clinics
            .Include(c => c.Doctors)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Clinic>> GetAllAsync()
    {
        return await _context.Clinics.ToListAsync();
    }

    public async Task<Clinic> AddAsync(Clinic clinic)
    {
        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();
        return clinic;
    }

    public async Task UpdateAsync(Clinic clinic)
    {
        _context.Clinics.Update(clinic);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic != null)
        {
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Clinics.AnyAsync(c => c.Id == id);
    }
}