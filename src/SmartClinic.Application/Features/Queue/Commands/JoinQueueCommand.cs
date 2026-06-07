using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Queue.Commands;

public record JoinQueueCommand(CreateQueueTicketDto Dto) : IRequest<QueueTicketDto>;

public class JoinQueueHandler : IRequestHandler<JoinQueueCommand, QueueTicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    private readonly INotificationService _notificationService;

    public JoinQueueHandler(
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

    public async Task<QueueTicketDto> Handle(JoinQueueCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Dto.PatientId);
        if (patient == null)
            throw new InvalidOperationException("Patient not found");

        var doctor = await _unitOfWork.Doctors.GetByIdWithClinicAsync(request.Dto.DoctorId);
        if (doctor == null)
            throw new InvalidOperationException("Doctor not found");

        if (!doctor.IsAvailable)
            throw new InvalidOperationException("Doctor is not available");

        var existingTicket = await _unitOfWork.QueueTickets.GetByPatientAndDoctorAsync(
            request.Dto.PatientId, request.Dto.DoctorId, TicketStatus.Waiting);

        if (existingTicket != null)
            throw new InvalidOperationException("Patient already has an active queue ticket for this doctor");

        var ticketNumber = await _unitOfWork.QueueTickets.GetNextTicketNumberAsync(request.Dto.DoctorId);
        var position = await _unitOfWork.QueueTickets.GetNextPositionAsync(request.Dto.DoctorId);

        var queueCount = await _unitOfWork.QueueTickets.GetActiveQueueCountAsync(request.Dto.DoctorId);
        var estimatedWaitMinutes = position * (doctor.EstimatedConsultationMinutes > 0
            ? doctor.EstimatedConsultationMinutes
            : doctor.Clinic.EstimatedWaitTimePerPatient);

        var ticket = new QueueTicket
        {
            TicketNumber = ticketNumber,
            Status = TicketStatus.Waiting,
            Position = position,
            CheckInTime = DateTime.UtcNow,
            EstimatedWaitMinutes = estimatedWaitMinutes,
            EstimatedStartTime = DateTime.UtcNow.AddMinutes(estimatedWaitMinutes),
            IsVip = request.Dto.IsVip || patient.IsVip,
            Notes = request.Dto.Notes,
            PatientId = request.Dto.PatientId,
            DoctorId = request.Dto.DoctorId,
            ClinicId = request.Dto.ClinicId
        };

        if (request.Dto.AppointmentId.HasValue)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Dto.AppointmentId.Value);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                await _unitOfWork.Appointments.UpdateAsync(appointment);
            }
        }

        var created = await _unitOfWork.QueueTickets.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _redisService.SetQueueTicketAsync(created);
        await _redisService.InvalidateDoctorQueueCacheAsync(request.Dto.DoctorId);

        await _notificationService.SendNotificationAsync(
            patient.Id,
            "Queue Ticket Created",
            $"You have joined the queue for Dr. {doctor.FullName}. Your position is #{position}. Estimated wait: {estimatedWaitMinutes} minutes.",
            NotificationType.QueuePositionUpdate);

        return _mapper.Map<QueueTicketDto>(created);
    }
}