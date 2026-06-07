using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;
using SmartClinic.Infrastructure.Data;

namespace SmartClinic.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly SmartClinicDbContext _context;

    public NotificationRepository(SmartClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByPatientIdAsync(int patientId, bool unreadOnly = false)
    {
        var query = _context.Notifications
            .Where(n => n.PatientId == patientId);

        if (unreadOnly)
            query = query.Where(n => n.Status != NotificationStatus.Read);

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetByQueueTicketIdAsync(int queueTicketId)
    {
        return await _context.Notifications
            .Where(n => n.QueueTicketId == queueTicketId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification> AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.Status = NotificationStatus.Read;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int patientId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.PatientId == patientId && n.Status != NotificationStatus.Read)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.Read;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int patientId)
    {
        return await _context.Notifications
            .Where(n => n.PatientId == patientId && n.Status != NotificationStatus.Read)
            .CountAsync();
    }
}