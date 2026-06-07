using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartClinic.Application.DTOs;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.IRepositories;
using SmartClinic.Domain.Entities;

namespace SmartClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClinicsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClinicDto>>> GetAll()
    {
        var clinics = await _unitOfWork.Clinics.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ClinicDto>>(clinics));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicDto>> GetById(int id)
    {
        var clinic = await _unitOfWork.Clinics.GetByIdWithDoctorsAsync(id);
        if (clinic == null)
            return NotFound();

        return Ok(_mapper.Map<ClinicDto>(clinic));
    }

    [HttpPost]
    public async Task<ActionResult<ClinicDto>> Create([FromBody] CreateClinicDto dto)
    {
        var clinic = _mapper.Map<Clinic>(dto);
        var created = await _unitOfWork.Clinics.AddAsync(clinic);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ClinicDto>(created));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClinicDto>> Update(int id, [FromBody] UpdateClinicDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
        if (clinic == null)
            return NotFound();

        clinic.Name = dto.Name;
        clinic.Address = dto.Address;
        clinic.Phone = dto.Phone;
        clinic.Email = dto.Email;
        clinic.IsActive = dto.IsActive;
        clinic.EstimatedWaitTimePerPatient = dto.EstimatedWaitTimePerPatient;
        clinic.WorkingHoursStart = dto.WorkingHoursStart;
        clinic.WorkingHoursEnd = dto.WorkingHoursEnd;

        await _unitOfWork.Clinics.UpdateAsync(clinic);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<ClinicDto>(clinic));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
        if (clinic == null)
            return NotFound();

        await _unitOfWork.Clinics.DeleteAsync(id);
        return NoContent();
    }
}