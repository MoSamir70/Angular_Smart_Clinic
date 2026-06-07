using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Features.Appointments.Commands;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;

namespace SmartClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public AppointmentsController(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _unitOfWork.Appointments.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetById(int id)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(id);
        if (appointment == null)
            return NotFound();

        return Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatient(int patientId)
    {
        var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId);
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDoctor(int doctorId)
    {
        var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId);
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByClinic(int clinicId)
    {
        var appointments = await _unitOfWork.Appointments.GetByClinicIdAsync(clinicId);
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var result = await _mediator.Send(new CreateAppointmentCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<AppointmentDto>> Cancel(int id, [FromBody] string? reason)
    {
        try
        {
            var result = await _mediator.Send(new CancelAppointmentCommand(id, reason));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/reschedule")]
    public async Task<ActionResult<AppointmentDto>> Reschedule(int id, [FromBody] DateTime newDateTime)
    {
        try
        {
            var result = await _mediator.Send(new RescheduleAppointmentCommand(id, newDateTime));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}