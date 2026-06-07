using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id);
    Task<IEnumerable<Notification>> GetByPatientIdAsync(int patientId, bool unreadOnly = false);
    Task<IEnumerable<Notification>> GetByQueueTicketIdAsync(int queueTicketId);
    Task<Notification> AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task DeleteAsync(int id);
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadAsync(int patientId);
    Task<int> GetUnreadCountAsync(int patientId);
}