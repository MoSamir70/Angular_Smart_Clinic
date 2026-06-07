using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;

namespace SmartClinic.Application.Features.Queue.Queries;

public record GetPatientQueueStatusQuery(int PatientId) : IRequest<PatientQueueStatusDto?>;

public class GetPatientQueueStatusHandler : IRequestHandler<GetPatientQueueStatusQuery, PatientQueueStatusDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPatientQueueStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PatientQueueStatusDto?> Handle(GetPatientQueueStatusQuery request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId);
        if (patient == null)
            throw new InvalidOperationException("Patient not found");

        var activeTickets = (await _unitOfWork.QueueTickets.GetByPatientIdAsync(request.PatientId))
            .Where(t => t.IsWaiting)
            .OrderBy(t => t.CheckInTime)
            .ToList();

        if (!activeTickets.Any())
            return null;

        var ticket = activeTickets.First();

        return new PatientQueueStatusDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Status = ticket.Status,
            Position = ticket.Position,
            EstimatedWaitMinutes = ticket.EstimatedWaitMinutes,
            EstimatedStartTime = ticket.EstimatedStartTime,
            DoctorId = ticket.DoctorId,
            DoctorName = ticket.Doctor?.FullName ?? string.Empty,
            ClinicId = ticket.ClinicId,
            ClinicName = ticket.Clinic?.Name ?? string.Empty
        };
    }
}