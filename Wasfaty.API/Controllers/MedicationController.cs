using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Medications;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]

[Authorize]

public class MedicationController : ControllerBase
{
    private readonly IMedicationService _medicationService;

    public MedicationController(IMedicationService medicationService)
    {
        _medicationService = medicationService;
    }

    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)] // استثناء
    // GET: api/medications
    [HttpGet("All", Name = "GetAllMedication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MedicationDto>>> GetAllMedication()
    {
        var medications = await _medicationService.GetAllAsync();

        if (medications == null || !medications.Any() || medications.Count() == 0)
        {
            return NotFound("No medications centers found.");
        }

        return Ok(medications);
    }



    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)] // استثناء
    // GET: api/medications/{id}
    [HttpGet("{id}", Name = "GetMedicationById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicationDto>> GetMedicationById(int id)
    {

        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var medication = await _medicationService.GetByIdAsync(id);
        if (medication == null)
        {
            return NotFound($"medication center with ID {id} not found.");
        }

        return Ok(medication);
    }

    [Authorize(Roles = Roles.Admin)]

    // POST: api/medications
    [HttpPost("CreateMedication")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicationDto>> CreateMedication([FromBody] CreateMedicationDto medicationDto)
    {

        if (medicationDto == null)
        {
            return BadRequest("Invalid medication center data.");
        }
        var createdMedication = await _medicationService.CreateAsync(medicationDto);
        if (createdMedication == null)
        {
            return BadRequest("Not created");
        }

        return CreatedAtRoute("GetMedicationById", new { id = createdMedication.Id }, createdMedication);
    }

    [Authorize(Roles = Roles.Admin)]

    // PUT: api/medications/{id}
    [HttpPut("{id}", Name = "UpdateMedication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicationDto>> UpdateMedication(int id, [FromBody] UpdateMedicationDto medicationDto)
    {

        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var updatedMedication = await _medicationService.UpdateAsync(id, medicationDto);
        if (updatedMedication == null)
        {
            return NotFound($"Medication center with ID {id} not found.");
        }
        return Ok(updatedMedication);
    }

    [Authorize(Roles = Roles.Admin)]

    // DELETE: api/medications/{id}
    [HttpDelete("{id}", Name = "DeleteMedication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMedication(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingMedication = await _medicationService.GetByIdAsync(id);
            if (existingMedication == null)
            {
                return NotFound($"Medication center with ID {id} not found.");
            }

            if (await _medicationService.DeleteAsync(id))
                return Ok($"Medication center ID {id} has been deleted.");
            else
                return NotFound($"Medication with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Medication with ID {id} not found. no rows deleted!");
        }
    
    }


    [HttpGet("GetMultipleByIds")]
    public async Task<IActionResult> GetMultipleByIds( string ids)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("يجب تقديم قائمة IDs");

            var idList = ids.Split(',').Select(int.Parse).ToList();

            var result = await _medicationService.GetMedicationsByIdsAsync(idList);


            if (result == null)
            {
                BadRequest("حصل طاء اثناء جلب الادوية");
            }


            return Ok(result);
               
        }
        catch (FormatException)
        {
            return BadRequest("تنسيق IDs غير صحيح");
        }
    }
}