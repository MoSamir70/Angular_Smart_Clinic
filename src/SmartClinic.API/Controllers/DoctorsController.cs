using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Features.Queue.Commands;
using SmartClinic.Application.Features.Queue.Queries;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;

namespace SmartClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<DoctorDto>>(doctors));
    }

    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetByClinic(int clinicId)
    {
        var doctors = await _unitOfWork.Doctors.GetByClinicIdAsync(clinicId);
        return Ok(_mapper.Map<IEnumerable<DoctorDto>>(doctors));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdWithClinicAsync(id);
        if (doctor == null)
            return NotFound();

        var dto = _mapper.Map<DoctorDto>(doctor);
        dto.QueueCount = await _unitOfWork.QueueTickets.GetActiveQueueCountAsync(id);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorDto dto)
    {
        var doctor = _mapper.Map<Doctor>(dto);
        var created = await _unitOfWork.Doctors.AddAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<DoctorDto>(created));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DoctorDto>> Update(int id, [FromBody] UpdateDoctorDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        doctor.FirstName = dto.FirstName;
        doctor.LastName = dto.LastName;
        doctor.Specialty = dto.Specialty;
        doctor.LicenseNumber = dto.LicenseNumber;
        doctor.Phone = dto.Phone;
        doctor.Email = dto.Email;
        doctor.IsAvailable = dto.IsAvailable;
        doctor.EstimatedConsultationMinutes = dto.EstimatedConsultationMinutes;

        await _unitOfWork.Doctors.UpdateAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<DoctorDto>(doctor));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        await _unitOfWork.Doctors.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/queue")]
    public async Task<ActionResult<QueueStatusDto>> GetQueueStatus(int id)
    {
        try
        {
            var queueStatus = await _mediator.Send(new GetDoctorQueueQuery(id));
            return Ok(queueStatus);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/call-next")]
    public async Task<ActionResult<QueueTicketDto?>> CallNextPatient(int id)
    {
        try
        {
            var result = await _mediator.Send(new CallNextPatientCommand(id));
            if (result == null)
                return Ok(new { message = "No patients in queue" });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}