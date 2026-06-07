using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly SmartClinicDbContext _context;

    public AppointmentRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments.FindAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Clinic)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Clinic)
            .Include(a => a.Clinic)
            .Include(a => a.QueueTicket)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Clinic)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Clinic)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.ClinicId == clinicId)
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateAsync(int doctorId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.ScheduledDateTime >= startOfDay && a.ScheduledDateTime < endOfDay)
            .OrderBy(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int clinicId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Clinic)
            .Where(a => a.ClinicId == clinicId && a.ScheduledDateTime >= DateTime.UtcNow && a.Status == AppointmentStatus.Scheduled)
            .OrderBy(a => a.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Appointments.AnyAsync(a => a.Id == id);
    }

    public async Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime dateTime, int? excludeId = null)
    {
        var windowStart = dateTime.AddMinutes(-30);
        var windowEnd = dateTime.AddMinutes(30);

        var query = _context.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.ScheduledDateTime >= windowStart
                && a.ScheduledDateTime <= windowEnd
                && a.Status != AppointmentStatus.Cancelled
                && a.Status != AppointmentStatus.Completed);

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}