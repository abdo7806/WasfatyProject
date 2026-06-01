using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wasfaty.Application.DTOs.Patients; // تأكد من تعديل المسار إذا لزم الأمر
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Infrastructure.Services;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Application.Interfaces.IServices;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IUserService _userService;
    private readonly IMedicalCenterService _medicalCenterService;
    private readonly IAuthorizationService _authorizationService;

    public DoctorController(IDoctorService doctorService, IUserService userService, IMedicalCenterService medicalCenterService, IAuthorizationService authorizationService)
    {
        _doctorService = doctorService;
        _userService = userService;
        _medicalCenterService = medicalCenterService;
        _authorizationService = authorizationService;
    }

    // GET: api/doctor
    [Authorize(Policy = "AdminRole")]
    [HttpGet("All", Name = "GetAllDoctors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        if (doctors == null || !doctors.Any())
        {
            return NotFound("No doctors found.");
        }
        return Ok(doctors);
    }

    // GET: api/doctor/{id}
    [HttpGet("{id}", Name = "GetDoctorById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> GetDoctorById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var doctor = await _doctorService.GetDoctorByIdAsync(id);

        if (doctor == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, doctor, "CanAccessDoctor");

        if (!auth.Succeeded)
            return Forbid();


        return Ok(doctor);
    }

    // POST: api/doctor
    [HttpPost("CreateDoctor")]
    [Authorize(Policy = "AdminRole")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateDoctor([FromBody] CreateDoctorDto doctorDto)
    {
        if (doctorDto == null)
        {
            return BadRequest("Invalid doctor data.");
        }

        MedicalCenterDto? medicalCenterDto = await _medicalCenterService.GetByIdAsync(doctorDto.MedicalCenterId);

        if (medicalCenterDto == null)
        {
            return BadRequest("Invalid medicalCenter data.");

        }

        var doctor = await _doctorService.CreateDoctorAsync(doctorDto);

        if (doctor == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetDoctorById", new { id = doctor.Id }, doctor);
    }


    // PUT: api/doctor/{id}
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpPut("{id}", Name = "UpdateDoctor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto doctorDto)
    {
        if ( id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);

        if (existingDoctor == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, existingDoctor, "CanEditDoctor");

        if (!auth.Succeeded)
            return Forbid();



        MedicalCenterDto? medicalCenterDto = await _medicalCenterService.GetByIdAsync(doctorDto.MedicalCenterId);

        if (medicalCenterDto == null)
        {
            return BadRequest("Invalid medicalCenterDto data.");

        }

        var updatedDoctor = await _doctorService.UpdateDoctorAsync(id, doctorDto);
        return Ok(updatedDoctor);
    }

    // DELETE: api/doctor/{id}
    [Authorize(Policy = "AdminRole")]
    [HttpDelete("{id}", Name = "DeleteDoctor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteDoctor(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
            if (existingDoctor == null)
            {
                return NotFound($"Doctor center with ID {id} not found.");
            }

            if (await _doctorService.DeleteDoctorAsync(id))
                return Ok($"Doctor center ID {id} has been deleted.");
            else
                return NotFound($"Doctor with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Doctor with ID {id} not found. no rows deleted!");
        }

  
    }

    // GET: api/doctor/{id}
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpGet("GetDoctorByUserId/{UserId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> GetDoctorByUserId(int UserId)
    {
        if (UserId < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var doctor = await _doctorService.GetDoctorByUserIdAsync(UserId);

        if (doctor == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, doctor, "CanAccessDoctor");

        if (!auth.Succeeded)
            return Forbid();

        return Ok(doctor);
    }

    [HttpGet("dashboard/{doctorId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard(int doctorId)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);

        if (doctor == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, doctor, "CanAccessDoctor");

        if (!auth.Succeeded)
            return Forbid();


        var dashboardData = await _doctorService.GetDashboardAsync(doctorId);
        if (dashboardData == null)
        {
            return NotFound("Dashboard data not found.");
        }
        return Ok(dashboardData);
    }


}