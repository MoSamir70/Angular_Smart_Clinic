using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Queue.Commands;

public record CancelFromQueueCommand(int TicketId, string? Reason = null) : IRequest<QueueTicketDto>;

public class CancelFromQueueHandler : IRequestHandler<CancelFromQueueCommand, QueueTicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    private readonly INotificationService _notificationService;

    public CancelFromQueueHandler(
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

    public async Task<QueueTicketDto> Handle(CancelFromQueueCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.QueueTickets.GetByIdWithDetailsAsync(request.TicketId);
        if (ticket == null)
            throw new InvalidOperationException("Queue ticket not found");

        if (ticket.Status == TicketStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed ticket");

        ticket.Cancel();
        if (!string.IsNullOrEmpty(request.Reason))
            ticket.Notes = (ticket.Notes ?? "") + $"\nCancellation reason: {request.Reason}";

        await _unitOfWork.QueueTickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _redisService.RemoveQueueTicketAsync(ticket.Id);
        await _redisService.InvalidateDoctorQueueCacheAsync(ticket.DoctorId);
        await _redisService.InvalidateClinicCacheAsync(ticket.ClinicId);

        return _mapper.Map<QueueTicketDto>(ticket);
    }
}