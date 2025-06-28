using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]

public class PrescriptionItemController : ControllerBase
{
    private readonly IPrescriptionItemService _prescriptionItemService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IMedicationService _medicationService;

    public PrescriptionItemController(IPrescriptionItemService prescriptionItemService,
        IPrescriptionService prescriptionService,
        IMedicationService medicationService)
    {
        _prescriptionItemService = prescriptionItemService;
        _prescriptionService = prescriptionService;
        _medicationService = medicationService;
    }

    // GET: api/prescriptionitems
    [HttpGet("All", Name = "GetAllPrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PrescriptionItemDto>>> GetAllPrescriptionItem()
    {
        var prescriptionItems = await _prescriptionItemService.GetAllAsync();
        if (prescriptionItems == null || !prescriptionItems.Any() || prescriptionItems.Count() == 0)
        {
            return NotFound("No doctors found.");
        }
        return Ok(prescriptionItems);
    }

    // GET: api/prescriptionitems/{id}
    [HttpGet("{id}", Name = "GetPrescriptionItemById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionItemDto>> GetPrescriptionItemById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var prescriptionItem = await _prescriptionItemService.GetByIdAsync(id);

        if (prescriptionItem == null)
        {
            return NotFound($"Doctor with ID {id} not found.");
        }
        return Ok(prescriptionItem);
    }
    //ارجاع كل الادويه لوصفخ معينه
    // GET: api/prescriptionitems/prescription/{id}
    [HttpGet("prescription/{id}", Name = "GetAllByPrescriptionId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PrescriptionItemDto>>> GetAllByPrescriptionId(int id)
    {
        if (id < 1)
        {
            return BadRequest("ID غير صالح.");
        }

        var prescriptionItems = await _prescriptionItemService.GetAllByPrescriptionId(id);
        if (prescriptionItems == null || !prescriptionItems.Any())
        {
            return NotFound("لا توجد أدوية لهذه الوصفة.");
        }
        return Ok(prescriptionItems);
    }

    // POST: api/prescriptionitems
    [HttpPost("CreatePrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionItemDto>> CreatePrescriptionItem([FromBody] CreatePrescriptionItemDto prescriptionItemDto)
    {
        if (prescriptionItemDto == null)
        {
            return BadRequest("Invalid PrescriptionItem data.");
        }


        var prescription = await _prescriptionService.GetAllAsync();

        if (!prescription.Any(d => d.Id == prescriptionItemDto.PrescriptionId))
        {
            return BadRequest("الوصفه مش موجودة");

        }

        /*var medication = await _medicationService.GetAllAsync();

        if (!medication.Any(d => d.Id == prescriptionItemDto.MedicationId))
        {
            return BadRequest("الدواى مش موجود");

        }*/


        var prescriptionItem = await _prescriptionItemService.CreateAsync(prescriptionItemDto);

        if (prescriptionItem == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetPrescriptionItemById", new { id = prescriptionItem.Id }, prescriptionItem);

       
    }

    // PUT: api/prescriptionitems/{id}
    [HttpPut("{id}", Name = "UpdatePrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrescriptionItemDto>> UpdatePrescriptionItem(int id, [FromBody] UpdatePrescriptionItemDto prescriptionItemDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

       /* var medication = await _medicationService.GetAllAsync();

        if (!medication.Any(d => d.Id == prescriptionItemDto.MedicationId))
        {
            return BadRequest("الدواى مش موجود");

        }*/

        var existingPrescriptionItem = await _prescriptionItemService.GetByIdAsync(id);
        if (existingPrescriptionItem == null)
        {
            return NotFound($"PrescriptionItem with ID {id} not found.");
        }

        var updatedPrescriptionItem = await _prescriptionItemService.UpdateAsync(id, prescriptionItemDto);
        if (updatedPrescriptionItem == null)
        {
            return NotFound("لم ينم التعديل");
        }
        return Ok(updatedPrescriptionItem);
    }

    // DELETE: api/prescriptionitems/{id}
    [HttpDelete("{id}", Name = "DeletePrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePrescriptionItem(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingPrescriptionItem = await _prescriptionItemService.GetByIdAsync(id);
            if (existingPrescriptionItem == null)
            {
                return NotFound($"PrescriptionItem center with ID {id} not found.");
            }

            if (await _prescriptionItemService.DeleteAsync(id))
                return Ok($"PrescriptionItem center ID {id} has been deleted.");
            else
                return NotFound($"PrescriptionItem with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"PrescriptionItem with ID {id} not found. no rows deleted!");
        }
    }
}