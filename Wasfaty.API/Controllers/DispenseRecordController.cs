using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]

public class DispenseRecordController : ControllerBase
{
    private readonly IDispenseRecordService _dispenseRecordService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IPharmacistService _pharmacistService;
    private readonly IPharmacyService _pharmacyService;

    public DispenseRecordController(IDispenseRecordService dispenseRecordService,
        IPrescriptionService prescriptionService,
        IPharmacistService pharmacistService,
        IPharmacyService pharmacyService)
    {
        _dispenseRecordService = dispenseRecordService;
        _prescriptionService = prescriptionService;
        _pharmacistService = pharmacistService;
        _pharmacyService = pharmacyService;
    }

    // GET: api/dispenserecords
    [HttpGet("All", Name = "GetAllDispenseRecor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DispenseRecordDto>>> GetAllDispenseRecor()
    {
        var dispenseRecords = await _dispenseRecordService.GetAllAsync();

        if (dispenseRecords == null || !dispenseRecords.Any() || dispenseRecords.Count() == 0)
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
        {
            return NotFound($"DispenseRecord with ID {id} not found.");
        }
        return Ok(dispenseRecord);
    }

    // POST: api/dispenserecords
    [HttpPost("CreateDispenseRecord")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DispenseRecordDto>> CreateDispenseRecord([FromBody] CreateDispenseRecordDto dispenseRecordDto)
    {
        if (dispenseRecordDto == null)
        {
            return BadRequest("Invalid DispenseRecord data.");
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

        var createdDispenseRecord = await _dispenseRecordService.CreateAsync(dispenseRecordDto);

        if (createdDispenseRecord == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetDispenseRecordById", new { id = createdDispenseRecord.Id }, createdDispenseRecord);

    }

    // PUT: api/dispenserecords/{id}
    [HttpPut("{id}", Name = "UpdateDispenseRecord")]
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

        var updatedDispenseRecord = await _dispenseRecordService.UpdateAsync(id, dispenseRecordDto);
        if (updatedDispenseRecord == null)
        {
            return NotFound("لم ينم التعديل");
        }

        return Ok(updatedDispenseRecord);
    }

    // DELETE: api/dispenserecords/{id}
    [HttpDelete("{id}", Name = "DeleteDispenseRecord")]
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




    


}