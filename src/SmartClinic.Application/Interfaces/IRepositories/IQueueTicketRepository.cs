using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Interfaces.IRepositories;

public interface IQueueTicketRepository
{
    Task<QueueTicket?> GetByIdAsync(int id);
    Task<QueueTicket?> GetByIdWithDetailsAsync(int id);
    Task<QueueTicket?> GetByPatientAndDoctorAsync(int patientId, int doctorId, TicketStatus? status = null);
    Task<IEnumerable<QueueTicket>> GetByDoctorIdAsync(int doctorId, TicketStatus? status = null);
    Task<IEnumerable<QueueTicket>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<QueueTicket>> GetByClinicIdAsync(int clinicId, TicketStatus? status = null);
    Task<IEnumerable<QueueTicket>> GetActiveQueueAsync(int doctorId);
    Task<IEnumerable<QueueTicket>> GetWaitingQueueAsync(int doctorId);
    Task<QueueTicket?> GetCurrentTicketAsync(int doctorId);
    Task<int> GetNextTicketNumberAsync(int doctorId);
    Task<int> GetNextPositionAsync(int doctorId);
    Task<QueueTicket> AddAsync(QueueTicket ticket);
    Task UpdateAsync(QueueTicket ticket);
    Task DeleteAsync(int id);
    Task<int> GetActiveQueueCountAsync(int doctorId);
}