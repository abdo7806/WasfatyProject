using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.Constants;
using System.Linq;

[Route("api/PatientController")]
[ApiController]

//[Authorize]

public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IUserService _userService;

    public PatientController(IPatientService patientService, IUserService userService)
    {
        _patientService = patientService;
        _userService = userService;
    }

  // [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)] // استثناء

    // GET: api/patient
    [HttpGet("All", Name = "GetAllPatients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllAsync();

        if (patients.Count() == 0)
        {
            return NotFound("No Patient Found!");
        }

     

        return Ok(patients);
    }

    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)] // استثناء
    // GET: api/patient/{id}
    [HttpGet("{id}", Name = "GetPatientById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientById(int id)
    {
        if (id < 1)
        {
            return BadRequest($"Not accepted ID {id}");
        }

        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return Ok(patient);
    }


    [Authorize(Roles = Roles.Admin)]

    // POST: api/patient
    //   [Authorize(Roles = "Admin")] // فقط المسؤولين يمكنهم إنشاء مرضى
    [HttpPost(Name = "CreatePatient")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreatePatient(CreatePatientDto patientDto)
    {
        if (patientDto == null)
        {
            return BadRequest("Invalid patient data.");
        }


        var p = await _patientService.GetAllAsync();

        if (p.Where(d => d.UserId == patientDto.UserId).Count() > 0)
        {
            return BadRequest("هاذا المستخدم مريض بالفعل");

        }

        UserDto user = await _userService.GetUserByIdAsync(patientDto.UserId);
        if (user == null)
        {
            return BadRequest("Invalid User data.");
        }

        if(user.Role != UserRoleEnum.Patient)
        {
            return BadRequest("لازم تكون صلاحيات المستخدم مريض");

        }

        var patient = await _patientService.CreateAsync(patientDto);
        if(patient == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetPatientById", new { id = patient.Id }, patient);
    }

    [Authorize(Roles = Roles.Admin)]

    // PUT: api/patient/{id}
    [HttpPut("{id}", Name = "UpdatePatient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
   // [Authorize(Roles = "Admin")] // فقط المسؤولين يمكنهم تحديث المرضى
    public async Task<ActionResult<PatientDto>> UpdatePatient(int id, UpdatePatientDto patientDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        PatientDto patient = await _patientService.UpdateAsync(id, patientDto);
        if (patient == null)
        {
            return BadRequest("The patient has not been modified");
        }
        return Ok(patient);
    }

    [Authorize(Roles = Roles.Admin)]

    // DELETE: api/patient/{id}
    [HttpDelete("{id}", Name = "DeletePatient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
   // [Authorize(Roles = "Admin")] // فقط المسؤولين يمكنهم حذف المرضى
    public async Task<ActionResult> DeletePatient(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            if (await _patientService.DeleteAsync(id))
                return Ok($"Patient with ID {id} has been deleted.");
            else
                return NotFound($"Patient with ID {id} not found. no rows deleted!");



        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Patient with ID {id} not found. no rows deleted!");
        }
  
    }



    // GET: api/patient
    [HttpGet("SearchPatients/{term}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PatientDto>>> SearchPatients(string term)
    {
        var patients = await _patientService.SearchPatients(term);

        if (patients.Count() == 0)
        {
            return NotFound("No Patient Found!");
        }
        return Ok(patients);
    }


   // [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)] // استثناء
    // GET: api/patient/{id}
    [HttpGet("GetPatientByUserId/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientByUserId(int userId)
    {
        if (userId < 1)
        {
            return BadRequest($"Not accepted userId {userId}");
        }

        var patient = await _patientService.GetPatientByUserIdAsync(userId);
        if (patient == null)
        {
            return NotFound($"Patient with ID {userId} not found.");
        }
        return Ok(patient);
    }




    [HttpGet("dashboard/{patientId}")]
    public async Task<IActionResult> GetDashboardData(int patientId)
    {
        try
        {

            if (patientId < 1)
            {
                return BadRequest($"Not accepted patientId {patientId}");
            }

            // جلب بيانات لوحة التحكم

            var dashboardData = await _patientService.GetDashboardDataAsync(patientId);

            if (dashboardData == null)
            {
                return NotFound($"dashboardData with ID {patientId} not found.");
            }
            return Ok(dashboardData);


         
        }
        catch (Exception ex)
        {
            return StatusCode(500, "حدث خطأ داخلي في الخادم");
        }
    }


}


