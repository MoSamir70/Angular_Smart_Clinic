using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Features.Appointments.Commands;
using SmartClinic.Application.Features.Queue.Commands;
using SmartClinic.Application.Features.Queue.Queries;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;

namespace SmartClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public PatientsController(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var patients = await _unitOfWork.Patients.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<PatientDto>>(patients));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetById(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        return Ok(_mapper.Map<PatientDto>(patient));
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
    {
        var patient = _mapper.Map<Patient>(dto);
        var created = await _unitOfWork.Patients.AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<PatientDto>(created));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> Update(int id, [FromBody] UpdatePatientDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Phone = dto.Phone;
        patient.Email = dto.Email;
        patient.Address = dto.Address;
        patient.IsVip = dto.IsVip;
        patient.Notes = dto.Notes;

        await _unitOfWork.Patients.UpdateAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<PatientDto>(patient));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        await _unitOfWork.Patients.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/queue-status")]
    public async Task<ActionResult<PatientQueueStatusDto>> GetQueueStatus(int id)
    {
        var status = await _mediator.Send(new GetPatientQueueStatusQuery(id));
        if (status == null)
            return NotFound("No active queue ticket found");

        return Ok(status);
    }

    [HttpPost("{patientId}/join-queue")]
    public async Task<ActionResult<QueueTicketDto>> JoinQueue(int patientId, [FromBody] CreateQueueTicketDto dto)
    {
        if (patientId != dto.PatientId)
            return BadRequest("Patient ID mismatch");

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
}