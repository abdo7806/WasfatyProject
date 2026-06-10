using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Numerics;
using System.Security.Claims;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;
    private readonly IAuthorizationService _authorizationService;

    //private string? GetUserId() =>
    //User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private int GetUserId() =>
    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private string? GetRole() =>
        User.FindFirst(ClaimTypes.Role)?.Value;


    public PrescriptionController(
        IPrescriptionService prescriptionService,
        IDoctorService doctorService,
        IPatientService patientService,
        IAuthorizationService authorizationService)
    {
        _prescriptionService = prescriptionService;
        _doctorService = doctorService;
        _patientService = patientService;
        _authorizationService = authorizationService;
    }

    // GET: api/prescriptions
    [Authorize(Policy = "AdminRole")]
    [HttpGet("All", Name = "GetAllPrescription")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PrescriptionDto>>> GetAllPrescription()
    {
        var prescriptions = await _prescriptionService.GetAllAsync();

        if (prescriptions == null || !prescriptions.Any() || prescriptions.Count() == 0)
        {
            return NotFound("No doctors found.");
        }
        return Ok(prescriptions);
    }

    // GET: api/prescriptions/{id}

    [Authorize]
    [HttpGet("{id}", Name = "GetPrescriptionById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> GetPrescriptionById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }
        var prescription = await _prescriptionService.GetByIdAsync(id);
        if (prescription == null)
        {
            return NotFound($"prescription with ID {id} not found.");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            User, prescription, "CanAccessPrescription");

        if (!authResult.Succeeded)
            return Forbid();


        return Ok(prescription);
    }

    // POST: api/prescriptions
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpPost("CreatePrescription")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> CreatePrescription([FromBody] CreatePrescriptionDto prescriptionDto)
    {

        if (prescriptionDto == null)
        {
            return BadRequest("Invalid prescription data.");
        }
        var role = GetRole();
        var userId = GetUserId();

        if (role == "Doctor")
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);

            if (doctor == null)
                return Forbid();

            if (prescriptionDto.DoctorId != doctor.Id)
                return Forbid();
        }
        var patients = await _patientService.GetAllAsync();

        if (!patients.Any(d => d.Id == prescriptionDto.PatientId))
        {
            return BadRequest("المريض مش موجود");

        }


        var createdPrescription = await _prescriptionService.CreateAsync(prescriptionDto);

        if (createdPrescription == null)
        {
            return BadRequest("Not created");
        }

        return CreatedAtRoute("GetPrescriptionById", new { id = createdPrescription.Id }, createdPrescription);
    }

    // PUT: api/prescriptions/{id}
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpPut("{id}", Name = "UpdatePrescription")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrescriptionDto>> UpdatePrescription(int id, [FromBody] CreatePrescriptionDto prescriptionDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }



        var patients = await _patientService.GetAllAsync();

        if (!patients.Any(d => d.Id == prescriptionDto.PatientId))
        {
            return BadRequest("المريض مش موجود");

        }
        var existingPrescription = await _prescriptionService.GetByIdAsync(id);
        if (existingPrescription == null)
        {
            return NotFound($"Prescription with ID {id} not found.");
        }
        var auth = await _authorizationService.AuthorizeAsync(
           User,
           existingPrescription,
           "CanEditPrescription");

        if (!auth.Succeeded)
            return Forbid();

        var updatedPrescription = await _prescriptionService.UpdateAsync(id, prescriptionDto);
        if (updatedPrescription == null)
        {
            return NotFound(".حدث خطاء اثنا التعديل");
        }
        return Ok(updatedPrescription);
    }

    // DELETE: api/prescriptions/{id}
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpDelete("{id}", Name = "DeletePrescription")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePrescription(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingPrescription = await _prescriptionService.GetByIdAsync(id);
            if (existingPrescription == null)
            {
                return NotFound($"Prescription center with ID {id} not found.");
            }
            var auth = await _authorizationService.AuthorizeAsync(
                User,
                existingPrescription,
                "CanEditPrescription");

            if (!auth.Succeeded)
                return Forbid();


            if (await _prescriptionService.DeleteAsync(id))
                return Ok($"Prescription center ID {id} has been deleted.");
            else
                return NotFound($"Prescription with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Prescription with ID {id} not found. no rows deleted!");
        }

    }

    //وضع علامة على أنه تم صرفه
    // PUT: api/dispenserecords/{id}
    [HttpPut("MarkAsDispensed/{id}")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> MarkAsDispensed(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }


        //var result = await _authorizationService.AuthorizeAsync(
        //  User, id, "CanDispensePrescription");

  
        var prescription = await _prescriptionService.GetByIdAsync(id);



        if (prescription == null)
            return NotFound();


        var auth = await _authorizationService.AuthorizeAsync(
           User,
           prescription,
           "CanDispensePrescription");

        if (!auth.Succeeded)
            return Forbid();

        prescription.IsDispensed = true;

        CreatePrescriptionDto existingprescriptionService = new CreatePrescriptionDto
        {
            DoctorId = prescription.DoctorId,
            PatientId = prescription.PatientId,
            IssuedDate = prescription.IssuedDate,
            IsDispensed = true// جعل الوصفة مصروفة
        };


        var updatedDispenseRecord = await _prescriptionService.UpdateAsync(id, existingprescriptionService);
        if (updatedDispenseRecord == null)
        {
            return NotFound("لم ينم التعديل");
        }

        return Ok(updatedDispenseRecord);
    }




    //// GET: api/GetByDoctorIdAsync
    //[Authorize(Policy = "DoctorRole")]
    //[HttpGet("GetByDoctorId/{doctorId}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<ActionResult<List<PrescriptionDto>>> GetByDoctorId(int doctorId)
    //{
    //    if (doctorId < 1)
    //    {
    //        return BadRequest("Invalid ID.");
    //    }

    //    var authResult = await _authorizationService.AuthorizeAsync(
    //         User, doctorId, "CanAccessPrescription");

    //    if (!authResult.Succeeded)
    //        return Forbid();

    //    var prescriptions = await _prescriptionService.GetByDoctorIdAsync(doctorId);

    //    if (prescriptions == null || !prescriptions.Any() || prescriptions.Count() == 0)
    //    {
    //        return NotFound("No doctors found.");
    //    }
    //    return Ok(prescriptions);
    //}

    [HttpGet("MyPrescriptions")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    public async Task<ActionResult<List<PrescriptionDto>>> GetMyPrescriptions()
    {
        var userId = GetUserId();
        var role = GetRole();

        if (role == "Doctor")
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (doctor == null) return Forbid();

            return Ok(await _prescriptionService.GetByDoctorIdAsync(doctor.Id));
        }

        if (role == "Patient")
        {
            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null) return Forbid();

            return Ok(await _prescriptionService.GetByPatientIdAsync(patient.Id));
        }


        return Forbid();
    }


    // GET: api/prescriptions
    //[Authorize]
    //[HttpGet("GetByPatientId/{PatientId}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<ActionResult<List<PrescriptionDto>>> GetByPatientId(int PatientId)
    //{

    //    if (PatientId < 1)
    //    {
    //        return BadRequest("Invalid ID.");
    //    }

    //    var authResult = await _authorizationService.AuthorizeAsync(
    //       User, PatientId, "CanAccessPrescription");


    //    if (!authResult.Succeeded)
    //        return Forbid();

    //    var prescriptions = await _prescriptionService.GetByPatientIdAsync(PatientId);

    //    if (prescriptions == null || !prescriptions.Any() || prescriptions.Count() == 0)
    //    {
    //        return NotFound("No doctors found.");
    //    }

    //    return Ok(prescriptions);
    //}

    [Authorize(Policy = "AdminRole")]
    [HttpGet("dashboard")]
    [EnableRateLimiting("DashboardPolicy")]
    public async Task<IActionResult> GetDashboardData()
    {
        try
        {
            // جلب بيانات لوحة التحكم
            var dashboardData = await _prescriptionService.GetDashboardDataAsync();

            if (dashboardData == null)
            {
                return NotFound($"dashboardData not found.");
            }

            return Ok(dashboardData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "حدث خطأ داخلي في الخادم");
        }
    }





    // GET: api/prescriptions
    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpGet("Pending", Name = "GetAllPrescriptionPending")]
    [EnableRateLimiting("DashboardPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PrescriptionDto>>> GetAllPrescriptionPending()
    {
        var prescriptions = await _prescriptionService.GetAllPrescriptionPendingAsync();

        if (prescriptions == null || !prescriptions.Any() || prescriptions.Count() == 0)
        {
            return NotFound("No doctors found.");
        }
        return Ok(prescriptions);
    }




    // الحصول على وصفات جديدة منذ آخر معرف
    [HttpGet("New/{lastPrescriptionId}")]
    [Authorize(Policy = "AdminOrDoctorRole")]
    [EnableRateLimiting("PollingPolicy")]
    public async Task<IActionResult> GetNewPrescriptions(int lastPrescriptionId)
    {

        var newPrescriptions = await _prescriptionService.GetNewPrescriptionsAsync(lastPrescriptionId);

        if (newPrescriptions == null || !newPrescriptions.Any() || newPrescriptions.Count() == 0)
        {
            return NotFound("No doctors found.");
        }
        return Ok(newPrescriptions);
    }




}