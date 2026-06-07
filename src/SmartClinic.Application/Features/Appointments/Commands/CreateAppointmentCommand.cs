using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Appointments.Commands;

public record CreateAppointmentCommand(CreateAppointmentDto Dto) : IRequest<AppointmentDto>;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IRedisService _redisService;

    public CreateAppointmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService,
        IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
        _redisService = redisService;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var existingPatient = await _unitOfWork.Patients.GetByIdAsync(request.Dto.PatientId);
        if (existingPatient == null)
            throw new InvalidOperationException("Patient not found");

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Dto.DoctorId);
        if (doctor == null)
            throw new InvalidOperationException("Doctor not found");

        var clinic = await _unitOfWork.Clinics.GetByIdAsync(request.Dto.ClinicId);
        if (clinic == null)
            throw new InvalidOperationException("Clinic not found");

        if (await _unitOfWork.Appointments.HasConflictingAppointmentAsync(
            request.Dto.DoctorId, request.Dto.ScheduledDateTime))
        {
            throw new InvalidOperationException("Doctor has a conflicting appointment at this time");
        }

        var appointment = new Appointment
        {
            ScheduledDateTime = request.Dto.ScheduledDateTime,
            Status = AppointmentStatus.Scheduled,
            Reason = request.Dto.Reason,
            Notes = request.Dto.Notes,
            PatientId = request.Dto.PatientId,
            DoctorId = request.Dto.DoctorId,
            ClinicId = request.Dto.ClinicId
        };

        var created = await _unitOfWork.Appointments.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.SendAppointmentConfirmationAsync(existingPatient.Id, created);

        return _mapper.Map<AppointmentDto>(created);
    }
}