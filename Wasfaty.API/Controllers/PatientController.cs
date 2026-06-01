using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wasfaty.Application.DTOs.Patients;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.Constants;
using System.Linq;
using Wasfaty.Application.Interfaces.IServices;
using System.Security.Claims;

[Route("api/PatientController")]
[ApiController]

[Authorize]

public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public PatientController(IPatientService patientService, IUserService userService, IAuthorizationService authorizationService)
    {
        _patientService = patientService;
        _userService = userService;
        _authorizationService = authorizationService;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException();

        return int.Parse(claim.Value);
    }
    // GET: api/patient
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpGet("All", Name = "GetAllPatients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllAsync();

        if (!patients.Any())
        {
            return NotFound("No Patient Found!");
        }

     

        return Ok(patients);
    }

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

        var auth = await _authorizationService.AuthorizeAsync(
       User, patient, "CanAccessPatient");


        if (!auth.Succeeded)
            return Forbid();

        return Ok(patient);
    }


    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpPost(Name = "CreatePatient")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreatePatient(CreatePatientDto patientDto)
    {
        if (patientDto == null)
        {
            return BadRequest("Invalid patient data.");
        }

        var patient = await _patientService.CreateAsync(patientDto);
        if(patient == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetPatientById", new { id = patient.Id }, patient);
    }

    // PUT: api/patient/{id}
    [Authorize]
    [HttpPut("{id}", Name = "UpdatePatient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(int id, UpdatePatientDto patientDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var existingPatient = await _patientService.GetByIdAsync(id);

        if (existingPatient == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, existingPatient, "CanEditPatient");

        if (!auth.Succeeded)
            return Forbid();

        PatientDto patient = await _patientService.UpdateAsync(id, patientDto);
        if (patient == null)
        {
            return BadRequest("The patient has not been modified");
        }
        return Ok(patient);
    }

    // DELETE: api/patient/{id}
    [Authorize(Policy = "AdminRole")]
    [HttpDelete("{id}", Name = "DeletePatient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePatient(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            var patient = await _patientService.GetByIdAsync(id);

            var auth = await _authorizationService.AuthorizeAsync(
            User, patient, "CanEditPatient");

            if (!auth.Succeeded)
                return Forbid();

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
    [Authorize(Policy = "AdminOrDoctorRole")]
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


    // GET: api/patient/{id}
    [Authorize]
    [HttpGet("GetPatientByUserId/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientByUserId(int userId)
    {
        if (userId < 1)
        {
            return BadRequest();
        }

        var patient = await _patientService.GetPatientByUserIdAsync(userId);
        if (patient == null)
        {
            return NotFound();
        }

        var auth = await _authorizationService.AuthorizeAsync(
    User, patient, "CanAccessPatient");

        if (!auth.Succeeded)
            return Forbid();


        return Ok(patient);
    }

    [Authorize]
    [HttpGet("MyProfile")]
    public async Task<ActionResult<PatientDto>> GetMyProfile()
    {
        var userId = GetUserId();

        var patient = await _patientService.GetPatientByUserIdAsync(userId);
        if (patient == null)
            return NotFound();

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

            var patient = await _patientService.GetByIdAsync(patientId);

            if (patient == null)
                return NotFound();

            var auth = await _authorizationService.AuthorizeAsync(
                User, patient, "CanAccessPatient");

            if (!auth.Succeeded)
                return Forbid();

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


