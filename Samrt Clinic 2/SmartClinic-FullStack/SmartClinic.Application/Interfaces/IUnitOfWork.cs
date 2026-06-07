using SmartClinic.Application.Interfaces.IRepositories;

namespace SmartClinic.Application.Interfaces;

public interface IUnitOfWork
{
    IClinicRepository Clinics { get; }
    IDoctorRepository Doctors { get; }
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    IQueueTicketRepository QueueTickets { get; }
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}