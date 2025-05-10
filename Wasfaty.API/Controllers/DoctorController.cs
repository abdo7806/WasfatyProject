using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wasfaty.Application.DTOs.Patients; // تأكد من تعديل المسار إذا لزم الأمر
using Wasfaty.Application.Interfaces;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Infrastructure.Services;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.Constants;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IUserService _userService;
    private readonly IMedicalCenterService _medicalCenterService;

    public DoctorController(IDoctorService doctorService, IUserService userService, IMedicalCenterService medicalCenterService)
    {
        _doctorService = doctorService;
        _userService = userService;
        _medicalCenterService = medicalCenterService;
    }

    // GET: api/doctor
    [HttpGet("All", Name = "GetAllDoctors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        if (doctors == null || !doctors.Any() || doctors.Count() ==0)
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
        {
            return NotFound($"Doctor with ID {id} not found.");
        }
        return Ok(doctor);
    }

    // POST: api/doctor
    [HttpPost("CreateDoctor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateDoctor([FromBody] CreateDoctorDto doctorDto)
    {
        if (doctorDto == null)
        {
            return BadRequest("Invalid doctor data.");
        }

        var doctors = await _doctorService.GetAllDoctorsAsync();

        if (doctors.Where(d => d.UserId == doctorDto.UserId).Count() > 0)
        {
            return BadRequest("هاذا المستخدم طبيب بالفعل");

        }

        UserDto user = await _userService.GetUserByIdAsync(doctorDto.UserId);
        if (user == null)
        {
            return BadRequest("Invalid User data.");
        }

        if (user.Role != UserRoleEnum.Doctor)
        {
            return BadRequest("لازم تكون صلاحيات المستخدم طبيب");

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

        MedicalCenterDto? medicalCenterDto = await _medicalCenterService.GetByIdAsync(doctorDto.MedicalCenterId);

        if (medicalCenterDto == null)
        {
            return BadRequest("Invalid medicalCenterDto data.");

        }

        var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
        if (existingDoctor == null)
        {
            return NotFound($"Doctor with ID {id} not found.");
        }

        await _doctorService.UpdateDoctorAsync(id, doctorDto);
        return Ok(existingDoctor);
    }

    // DELETE: api/doctor/{id}
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

            var existingMedicalCenter = await _doctorService.GetDoctorByIdAsync(id);
            if (existingMedicalCenter == null)
            {
                return NotFound($"Medical center with ID {id} not found.");
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
}