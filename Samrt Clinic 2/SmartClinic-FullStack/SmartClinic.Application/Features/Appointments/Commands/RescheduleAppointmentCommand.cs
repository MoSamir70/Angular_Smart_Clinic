using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Appointments.Commands;

public record RescheduleAppointmentCommand(int AppointmentId, DateTime NewDateTime) : IRequest<AppointmentDto>;

public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public RescheduleAppointmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<AppointmentDto> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(request.AppointmentId);
        if (appointment == null)
            throw new InvalidOperationException("Appointment not found");

        if (!appointment.CanReschedule())
            throw new InvalidOperationException("Appointment cannot be rescheduled");

        if (await _unitOfWork.Appointments.HasConflictingAppointmentAsync(
            appointment.DoctorId, request.NewDateTime, appointment.Id))
        {
            throw new InvalidOperationException("Doctor has a conflicting appointment at this new time");
        }

        var oldDateTime = appointment.ScheduledDateTime;
        appointment.ScheduledDateTime = request.NewDateTime;
        appointment.Status = AppointmentStatus.Scheduled;

        await _unitOfWork.Appointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.SendNotificationAsync(
            appointment.PatientId,
            "Appointment Rescheduled",
            $"Your appointment with Dr. {appointment.Doctor.FullName} has been rescheduled from {oldDateTime:g} to {request.NewDateTime:g}.",
            NotificationType.RescheduleConfirmation);

        return _mapper.Map<AppointmentDto>(appointment);
    }
}