using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]

public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;

    public PrescriptionController(IPrescriptionService prescriptionService, 
        IDoctorService doctorService, IPatientService patientService)
    {
        _prescriptionService = prescriptionService;
        _doctorService = doctorService;
        _patientService = patientService;
    }

    // GET: api/prescriptions
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
        return Ok(prescription);
    }

    // POST: api/prescriptions
    [HttpPost("CreatePrescription")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> CreatePrescription([FromBody] CreatePrescriptionDto prescriptionDto)
    {

        if (prescriptionDto == null)
        {
            return BadRequest("Invalid prescription data.");
        }

        var doctors = await _doctorService.GetAllDoctorsAsync();

        if (!doctors.Any(d=> d.Id == prescriptionDto.DoctorId))
        {
            return BadRequest("الطبيب مش موجود");

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
    [HttpPut("{id}", Name = "UpdatePrescription")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrescriptionDto>> UpdatePrescription(int id, [FromBody] CreatePrescriptionDto prescriptionDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var doctors = await _doctorService.GetAllDoctorsAsync();

        if (!doctors.Any(d => d.Id == prescriptionDto.DoctorId))
        {
            return BadRequest("الطبيب مش موجود");

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

        var updatedPrescription = await _prescriptionService.UpdateAsync(id, prescriptionDto);
        if (updatedPrescription == null)
        {
            return NotFound(".حدث خطاء اثنا التعديل");
        }
        return Ok(updatedPrescription);
    }

    // DELETE: api/prescriptions/{id}
    [HttpDelete("{id}", Name = "DeletePrescription")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> MarkAsDispensed(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var prescriptionService = await _prescriptionService.GetByIdAsync(id);



        if (prescriptionService == null)
        {
            return BadRequest("الوصفه مش موجودة");

        }

        CreatePrescriptionDto existingprescriptionService = new CreatePrescriptionDto
        {
            DoctorId = prescriptionService.DoctorId,
            PatientId = prescriptionService.PatientId,
            IssuedDate = prescriptionService.IssuedDate,
            IsDispensed = true// جعل الوصفة مصروفة
        };

        // existingDispenseRecord.isDispensed = true;
        var updatedDispenseRecord = await _prescriptionService.UpdateAsync(id, existingprescriptionService);
        if (updatedDispenseRecord == null)
        {
            return NotFound("لم ينم التعديل");
        }

        return Ok(updatedDispenseRecord);
    }



}