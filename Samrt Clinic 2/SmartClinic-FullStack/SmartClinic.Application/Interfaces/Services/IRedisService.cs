using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.Services;

public interface IRedisService
{
    Task SetQueueTicketAsync(QueueTicket ticket);
    Task<QueueTicket?> GetQueueTicketAsync(int ticketId);
    Task RemoveQueueTicketAsync(int ticketId);
    Task UpdateQueuePositionAsync(int doctorId, int ticketId, int newPosition);
    Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId);
    Task SetNextPatientCacheAsync(int doctorId, int? ticketId);
    Task<int?> GetNextPatientCacheAsync(int doctorId);
    Task SetClinicQueueCacheAsync(int clinicId, IEnumerable<QueueTicket> tickets);
    Task<IEnumerable<QueueTicket>> GetClinicQueueCacheAsync(int clinicId);
    Task InvalidateClinicCacheAsync(int clinicId);
    Task InvalidateDoctorQueueCacheAsync(int doctorId);
    Task CacheAppointmentAsync(Appointment appointment);
    Task<Appointment?> GetCachedAppointmentAsync(int appointmentId);
    Task InvalidateAppointmentCacheAsync(int appointmentId);
}