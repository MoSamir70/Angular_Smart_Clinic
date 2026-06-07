using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Queue.Commands;

public record CallNextPatientCommand(int DoctorId, int? CalledByDoctorId = null) : IRequest<QueueTicketDto?>;

public class CallNextPatientHandler : IRequestHandler<CallNextPatientCommand, QueueTicketDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    private readonly INotificationService _notificationService;

    public CallNextPatientHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRedisService redisService,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
        _notificationService = notificationService;
    }

    public async Task<QueueTicketDto?> Handle(CallNextPatientCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdWithClinicAsync(request.DoctorId);
        if (doctor == null)
            throw new InvalidOperationException("Doctor not found");

        var nextTicket = (await _unitOfWork.QueueTickets.GetWaitingQueueAsync(request.DoctorId))
            .OrderBy(t => t.IsVip ? 0 : 1)
            .ThenBy(t => t.Position)
            .FirstOrDefault();

        if (nextTicket == null)
            return null;

        nextTicket.Call();
        nextTicket.CalledByDoctorId = request.CalledByDoctorId ?? request.DoctorId;

        await _unitOfWork.QueueTickets.UpdateAsync(nextTicket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _redisService.SetQueueTicketAsync(nextTicket);
        await _redisService.SetNextPatientCacheAsync(request.DoctorId, nextTicket.Id);
        await _redisService.InvalidateDoctorQueueCacheAsync(request.DoctorId);

        await _notificationService.SendDoctorCalledAsync(nextTicket.PatientId, nextTicket);
        await _redisService.InvalidateClinicCacheAsync(doctor.ClinicId);

        return _mapper.Map<QueueTicketDto>(nextTicket);
    }
}