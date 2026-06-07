using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Queue.Commands;

public record CompletePatientCommand(int TicketId) : IRequest<QueueTicketDto>;

public class CompletePatientHandler : IRequestHandler<CompletePatientCommand, QueueTicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    private readonly INotificationService _notificationService;

    public CompletePatientHandler(
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

    public async Task<QueueTicketDto> Handle(CompletePatientCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.QueueTickets.GetByIdWithDetailsAsync(request.TicketId);
        if (ticket == null)
            throw new InvalidOperationException("Queue ticket not found");

        if (ticket.Status != TicketStatus.Called && ticket.Status != TicketStatus.InProgress)
            throw new InvalidOperationException("Ticket is not in a valid state to be completed");

        ticket.Complete();

        await _unitOfWork.QueueTickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _redisService.RemoveQueueTicketAsync(ticket.Id);
        await _redisService.InvalidateDoctorQueueCacheAsync(ticket.DoctorId);
        await _redisService.InvalidateClinicCacheAsync(ticket.ClinicId);

        var remainingTickets = await _unitOfWork.QueueTickets.GetActiveQueueAsync(ticket.DoctorId);
        foreach (var remaining in remainingTickets)
        {
            remaining.EstimatedWaitMinutes -= ticket.Doctor?.EstimatedConsultationMinutes ?? 15;
            remaining.EstimatedStartTime = DateTime.UtcNow.AddMinutes(remaining.EstimatedWaitMinutes);
            await _unitOfWork.QueueTickets.UpdateAsync(remaining);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var nextPatient = remainingTickets.OrderBy(t => t.Position).FirstOrDefault();
        if (nextPatient != null)
        {
            await _redisService.SetNextPatientCacheAsync(ticket.DoctorId, nextPatient.Id);
            await _notificationService.SendYouAreNextAsync(nextPatient.PatientId, nextPatient);
        }
        else
        {
            await _redisService.SetNextPatientCacheAsync(ticket.DoctorId, null);
        }

        return _mapper.Map<QueueTicketDto>(ticket);
    }
}