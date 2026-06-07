using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class QueueTicketRepository : IQueueTicketRepository
{
    private readonly SmartClinicDbContext _context;

    public QueueTicketRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<QueueTicket?> GetByIdAsync(int id)
    {
        return await _context.QueueTickets.FindAsync(id);
    }

    public async Task<QueueTicket?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.QueueTickets
            .Include(q => q.Patient)
            .Include(q => q.Doctor)
            .ThenInclude(d => d.Clinic)
            .Include(q => q.Clinic)
            .Include(q => q.Appointment)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<QueueTicket?> GetByPatientAndDoctorAsync(int patientId, int doctorId, TicketStatus? status = null)
    {
        var query = _context.QueueTickets
            .Include(q => q.Doctor)
            .Include(q => q.Clinic)
            .Where(q => q.PatientId == patientId && q.DoctorId == doctorId);

        if (status.HasValue)
            query = query.Where(q => q.Status == status.Value);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<QueueTicket>> GetByDoctorIdAsync(int doctorId, TicketStatus? status = null)
    {
        var query = _context.QueueTickets
            .Include(q => q.Patient)
            .Include(q => q.Clinic)
            .Where(q => q.DoctorId == doctorId);

        if (status.HasValue)
            query = query.Where(q => q.Status == status.Value);

        return await query.OrderBy(q => q.Position).ToListAsync();
    }

    public async Task<IEnumerable<QueueTicket>> GetByClinicIdAsync(int clinicId, TicketStatus? status = null)
    {
        var query = _context.QueueTickets
            .Include(q => q.Patient)
            .Include(q => q.Doctor)
            .Where(q => q.ClinicId == clinicId);

        if (status.HasValue)
            query = query.Where(q => q.Status == status.Value);

        return await query.OrderBy(q => q.CheckInTime).ToListAsync();
    }

    public async Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId)
    {
        return await _context.QueueTickets
            .Include(q => q.Patient)
            .Include(q => q.Doctor)
            .Where(q => q.DoctorId == doctorId && q.Status != TicketStatus.Completed && q.Status != TicketStatus.Cancelled && q.Status != TicketStatus.NoShow)
            .OrderBy(q => q.IsVip ? 0 : 1)
            .ThenBy(q => q.Position)
            .ToListAsync();
    }

    public async Task<IEnumerable<QueueTicket>> GetWaitingQueueAsync(int doctorId)
    {
        return await _context.QueueTickets
            .Include(q => q.Patient)
            .Include(q => q.Doctor)
            .Include(q => q.Clinic)
            .Where(q => q.DoctorId == doctorId && (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.Called))
            .OrderBy(q => q.IsVip ? 0 : 1)
            .ThenBy(q => q.Position)
            .ToListAsync();
    }

    public async Task<QueueTicket?> GetCurrentTicketAsync(int doctorId)
    {
        return await _context.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.DoctorId == doctorId && q.Status == TicketStatus.Called)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextTicketNumberAsync(int doctorId)
    {
        var lastTicket = await _context.QueueTickets
            .Where(q => q.DoctorId == doctorId)
            .OrderByDescending(q => q.TicketNumber)
            .FirstOrDefaultAsync();

        return (lastTicket?.TicketNumber ?? 0) + 1;
    }

    public async Task<int> GetNextPositionAsync(int doctorId)
    {
        var lastInQueue = await _context.QueueTickets
            .Where(q => q.DoctorId == doctorId && q.Status != TicketStatus.Completed && q.Status != TicketStatus.Cancelled && q.Status != TicketStatus.NoShow)
            .OrderByDescending(q => q.Position)
            .FirstOrDefaultAsync();

        return (lastInQueue?.Position ?? 0) + 1;
    }

    public async Task<QueueTicket> AddAsync(QueueTicket ticket)
    {
        _context.QueueTickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task UpdateAsync(QueueTicket ticket)
    {
        _context.QueueTickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ticket = await _context.QueueTickets.FindAsync(id);
        if (ticket != null)
        {
            _context.QueueTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetActiveQueueCountAsync(int doctorId)
    {
        return await _context.QueueTickets
            .Where(q => q.DoctorId == doctorId && q.Status != TicketStatus.Completed && q.Status != TicketStatus.Cancelled && q.Status != TicketStatus.NoShow)
            .CountAsync();
    }

    public async Task<IEnumerable<QueueTicket>> GetByPatientIdAsync(int patientId)
    {
        return await _context.QueueTickets
            .Include(q => q.Doctor)
            .Include(q => q.Clinic)
            .Where(q => q.PatientId == patientId)
            .ToListAsync();
    }
}