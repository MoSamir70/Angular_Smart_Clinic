using Microsoft.EntityFrameworkCore.Storage;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Infrastructure.Data;
using SmartClinic.Infrastructure.Repositories;

namespace SmartClinic.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartClinicDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(SmartClinicDbContext context)
    {
        _context = context;
    }

    public IClinicRepository Clinics => new ClinicRepository(_context);
    public IDoctorRepository Doctors => new DoctorRepository(_context);
    public IPatientRepository Patients => new PatientRepository(_context);
    public IAppointmentRepository Appointments => new AppointmentRepository(_context);
    public IQueueTicketRepository QueueTickets => new QueueTicketRepository(_context);
    public INotificationRepository Notifications => new NotificationRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}