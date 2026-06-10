using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]
//[Authorize(Roles = Roles.Admin + "," + Roles.Pharmacist)] // استثناء
[Authorize]
public class DispenseRecordController : ControllerBase
{
    private readonly IDispenseRecordService _dispenseRecordService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IPharmacistService _pharmacistService;
    private readonly IPharmacyService _pharmacyService;
    private readonly IAuthorizationService _authorizationService;

    public DispenseRecordController(IDispenseRecordService dispenseRecordService,
        IPrescriptionService prescriptionService,
        IPharmacistService pharmacistService,
        IPharmacyService pharmacyService,
        IAuthorizationService authorizationService)
    {
        _dispenseRecordService = dispenseRecordService;
        _prescriptionService = prescriptionService;
        _pharmacistService = pharmacistService;
        _pharmacyService = pharmacyService;
        _authorizationService = authorizationService;
    }

    private int GetUserId() =>
      int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // GET: api/dispenserecords
    [Authorize(Policy = "AdminRole")]
    [HttpGet("All", Name = "GetAllDispenseRecor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DispenseRecordDto>>> GetAllDispenseRecor()
    {
        var dispenseRecords = await _dispenseRecordService.GetAllAsync();

        if (!dispenseRecords.Any())
        {
            return NotFound("No dispenseRecords found.");
        }

        return Ok(dispenseRecords);
    }

    // GET: api/dispenserecords/{id}
    [HttpGet("{id}", Name = "GetDispenseRecordById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DispenseRecordDto>> GetDispenseRecordById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var dispenseRecord = await _dispenseRecordService.GetByIdAsync(id);

        if (dispenseRecord == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, dispenseRecord, "CanAccessDispenseRecord");

        if (!auth.Succeeded)
            return Forbid();

        return Ok(dispenseRecord);
    }

    // POST: api/dispenserecords
    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpPost("CreateDispenseRecord")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DispenseRecordDto>> CreateDispenseRecord([FromBody] CreateDispenseRecordDto dispenseRecordDto)
    {

        if (dispenseRecordDto == null)
        {
            return BadRequest("Invalid DispenseRecord data.");
        }

        var userId = GetUserId();

        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Admin")
        {
            var pharmacist = await _pharmacistService.GetByIdAsync(dispenseRecordDto.PharmacistId);

            if (pharmacist == null)
            {
                return BadRequest("الصيدلي مش موجود");

            }

            dispenseRecordDto.PharmacistId = pharmacist.Id;

        }

        if (role == "Pharmacist")
        {
            var pharmacist = await _pharmacistService
                .GetPharmacistByUserIdAsync(userId);

            if (pharmacist == null)
                return Unauthorized();

            dispenseRecordDto.PharmacistId = pharmacist.Id;

        }

        var prescription = await _prescriptionService.GetByIdAsync(dispenseRecordDto.PrescriptionId);

        if (prescription == null)
        {
            return BadRequest("الوصفه مش موجودة");

        }


        if (prescription.IsDispensed)
            return BadRequest("تم صرف الوصفة مسبقاً");



        var pharmacy = await _pharmacyService.GetByIdAsync(dispenseRecordDto.PharmacyId);

        if (pharmacy == null)
        {
            return BadRequest("الصيدلية مش موجودة");

        }

        dispenseRecordDto.PharmacyId = pharmacy.Id;

        var createdDispenseRecord = await _dispenseRecordService.CreateAsync(dispenseRecordDto);

        if (createdDispenseRecord == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetDispenseRecordById", new { id = createdDispenseRecord.Id }, createdDispenseRecord);

    }

    // PUT: api/dispenserecords/{id}
    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpPut("{id}", Name = "UpdateDispenseRecord")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DispenseRecordDto>> UpdateDispenseRecord(int id, [FromBody] CreateDispenseRecordDto dispenseRecordDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }


        var prescription = await _prescriptionService.GetByIdAsync(dispenseRecordDto.PrescriptionId);

        if (prescription == null)
        {
            return BadRequest("الوصفه مش موجودة");

        }

        var pharmacist = await _pharmacistService.GetByIdAsync(dispenseRecordDto.PharmacistId);

        if (pharmacist == null)
        {
            return BadRequest("الصيدلي مش موجود");

        }

        var pharmacy = await _pharmacyService.GetByIdAsync(dispenseRecordDto.PharmacyId);

        if (pharmacy == null)
        {
            return BadRequest("الصيدلية مش موجودة");

        }

        var existingDispenseRecord = await _dispenseRecordService.GetByIdAsync(id);
        if (existingDispenseRecord == null)
        {
            return NotFound($"DispenseRecord with ID {id} not found.");
        }

        var auth = await _authorizationService.AuthorizeAsync(
          User, existingDispenseRecord, "CanEditDispenseRecord");

        if (!auth.Succeeded)
            return Forbid();

        var updatedDispenseRecord = await _dispenseRecordService.UpdateAsync(id, dispenseRecordDto);
        if (updatedDispenseRecord == null)
        {
            return NotFound("لم ينم التعديل");
        }

        return Ok(updatedDispenseRecord);
    }

    // DELETE: api/dispenserecords/{id}
    [Authorize(Policy = "AdminRole")]
    [HttpDelete("{id}", Name = "DeleteDispenseRecord")]
    [EnableRateLimiting("WriteOperationsPolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDispenseRecord(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingDispenseRecord = await _dispenseRecordService.GetByIdAsync(id);
            if (existingDispenseRecord == null)
            {
                return NotFound($"DispenseRecord center with ID {id} not found.");
            }

            var auth = await _authorizationService.AuthorizeAsync(
     User, existingDispenseRecord, "CanEditDispenseRecord");

            if (!auth.Succeeded)
                return Forbid();

            if (await _dispenseRecordService.DeleteAsync(id))
            {
                var prescriptionService = await _prescriptionService.GetByIdAsync(existingDispenseRecord.PrescriptionId);



                if (prescriptionService == null)
                {
                    return BadRequest("الوصفه مش موجودة");

                }

                CreatePrescriptionDto existingprescriptionService = new CreatePrescriptionDto
                {
                    DoctorId = prescriptionService.DoctorId,
                    PatientId = prescriptionService.PatientId,
                    IssuedDate = prescriptionService.IssuedDate,
                    IsDispensed = false// جعل الوصفة مصروفة
                };

                // existingDispenseRecord.isDispensed = true;
                var updatedDispenseRecord = await _prescriptionService.UpdateAsync(existingDispenseRecord.PrescriptionId, existingprescriptionService);
                return Ok($"DispenseRecord center ID {id} has been deleted.");
            }
            else
                return NotFound($"DispenseRecord with ID {id} not found. no rows deleted!");




        }
        catch (KeyNotFoundException)
        {
            return NotFound($"DispenseRecord with ID {id} not found. no rows deleted!");
        }

    }




    [HttpGet("GetAllDispenseRecor/{PharmacyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DispenseRecordDto>>> GetByPharmacyId(int PharmacyId)
    {
        var dispenseRecords = await _dispenseRecordService.GetByPharmacyIdAsync(PharmacyId);

        if (dispenseRecords == null || !dispenseRecords.Any() || dispenseRecords.Count() == 0)
        {
            return NotFound("No dispenseRecords found.");
        }

        return Ok(dispenseRecords);
    }





}