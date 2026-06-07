using AutoMapper;
using MediatR;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Features.Queue.Queries;

public record GetDoctorQueueQuery(int DoctorId) : IRequest<QueueStatusDto>;

public class GetDoctorQueueHandler : IRequestHandler<GetDoctorQueueQuery, QueueStatusDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDoctorQueueHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QueueStatusDto> Handle(GetDoctorQueueQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdWithClinicAsync(request.DoctorId);
        if (doctor == null)
            throw new InvalidOperationException("Doctor not found");

        var waitingTickets = (await _unitOfWork.QueueTickets.GetWaitingQueueAsync(request.DoctorId))
            .OrderBy(t => t.IsVip ? 0 : 1)
            .ThenBy(t => t.Position)
            .ToList();

        var queueCount = waitingTickets.Count;
        var avgWaitMinutes = queueCount > 0
            ? waitingTickets.Average(t => t.EstimatedWaitMinutes)
            : 0;

        var currentTicket = waitingTickets.FirstOrDefault(t => t.Status == TicketStatus.Called);
        var nextPatient = waitingTickets.FirstOrDefault(t => t.Status == TicketStatus.Waiting);

        return new QueueStatusDto
        {
            DoctorId = doctor.Id,
            DoctorName = doctor.FullName,
            DoctorSpecialty = doctor.Specialty,
            QueueCount = queueCount,
            CurrentTicketId = currentTicket?.Id,
            CurrentPatientName = currentTicket?.Patient?.FullName,
            NextPatient = nextPatient != null ? _mapper.Map<QueueTicketDto>(nextPatient) : null,
            WaitingList = _mapper.Map<List<QueueTicketDto>>(waitingTickets),
            AverageWaitMinutes = (int)avgWaitMinutes
        };
    }
}