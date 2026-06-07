using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Appointments.Commands;

public record CancelAppointmentCommand(int AppointmentId, string? Reason = null) : IRequest<AppointmentDto>;

public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CancelAppointmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<AppointmentDto> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(request.AppointmentId);
        if (appointment == null)
            throw new InvalidOperationException("Appointment not found");

        if (!appointment.CanCancel())
            throw new InvalidOperationException("Appointment cannot be cancelled. Must be cancelled at least 2 hours before scheduled time.");

        appointment.MarkAsCancelled();
        if (!string.IsNullOrEmpty(request.Reason))
            appointment.Notes = (appointment.Notes ?? "") + $"\nCancellation reason: {request.Reason}";

        await _unitOfWork.Appointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.SendNotificationAsync(
            appointment.PatientId,
            "Appointment Cancelled",
            $"Your appointment with Dr. {appointment.Doctor.FullName} on {appointment.ScheduledDateTime:g} has been cancelled.",
            NotificationType.CancellationConfirmation);

        return _mapper.Map<AppointmentDto>(appointment);
    }
}