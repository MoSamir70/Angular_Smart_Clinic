using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Features.Queue.Commands;
using SmartClinic.Application.Features.Queue.Queries;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;

namespace SmartClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public QueueController(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<QueueStatusDto>> GetDoctorQueue(int doctorId)
    {
        try
        {
            var queue = await _mediator.Send(new GetDoctorQueueQuery(doctorId));
            return Ok(queue);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<IEnumerable<QueueTicketDto>>> GetClinicQueue(int clinicId)
    {
        var tickets = await _unitOfWork.QueueTickets.GetByClinicIdAsync(clinicId);
        return Ok(_mapper.Map<IEnumerable<QueueTicketDto>>(tickets));
    }

    [HttpGet("ticket/{ticketId}")]
    public async Task<ActionResult<QueueTicketDto>> GetTicket(int ticketId)
    {
        var ticket = await _unitOfWork.QueueTickets.GetByIdWithDetailsAsync(ticketId);
        if (ticket == null)
            return NotFound();

        return Ok(_mapper.Map<QueueTicketDto>(ticket));
    }

    [HttpPost("join")]
    public async Task<ActionResult<QueueTicketDto>> JoinQueue([FromBody] CreateQueueTicketDto dto)
    {
        try
        {
            var result = await _mediator.Send(new JoinQueueCommand(dto));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{ticketId}/complete")]
    public async Task<ActionResult<QueueTicketDto>> CompleteTicket(int ticketId)
    {
        try
        {
            var result = await _mediator.Send(new CompletePatientCommand(ticketId));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{ticketId}/cancel")]
    public async Task<ActionResult<QueueTicketDto>> CancelTicket(int ticketId, [FromBody] string? reason)
    {
        try
        {
            var result = await _mediator.Send(new CancelFromQueueCommand(ticketId, reason));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}