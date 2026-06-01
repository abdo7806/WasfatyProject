using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]
[Authorize]

public class PrescriptionItemController : ControllerBase
{
    private readonly IPrescriptionItemService _prescriptionItemService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IAuthorizationService _authorizationService;

    public PrescriptionItemController(IPrescriptionItemService prescriptionItemService,
        IPrescriptionService prescriptionService,
        IAuthorizationService authorizationService)
    {
        _prescriptionItemService = prescriptionItemService;
        _prescriptionService = prescriptionService;
        _authorizationService = authorizationService;
    }

    // GET: api/prescriptionitems
    [Authorize(Policy = "AdminRole")]
    [HttpGet("All", Name = "GetAllPrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PrescriptionItemDto>>> GetAllPrescriptionItem()
    {
        var prescriptionItems = await _prescriptionItemService.GetAllAsync();
        if (prescriptionItems == null || !prescriptionItems.Any())
        {
            return NotFound("No prescription items found.");
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

        var prescription = await _prescriptionService.GetByIdAsync(prescriptionItem.PrescriptionId);
        if (prescription == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, prescription, "CanAccessPrescription");

        if (!auth.Succeeded)
            return Forbid();


        return Ok(prescriptionItem);
    }
    //ارجاع كل الادويه لوصفخ معينه
    // GET: api/prescriptionitems/prescription/{id}
    [HttpGet("prescription/{id}", Name = "GetAllByPrescriptionId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PrescriptionItemDto>>> GetAllByPrescriptionId(int prescriptionId)
    {
        if (prescriptionId < 1)
        {
            return BadRequest("ID غير صالح.");
        }

        var prescription = await _prescriptionService.GetByIdAsync(prescriptionId);
        if (prescription == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, prescription, "CanAccessPrescription");

        if (!auth.Succeeded)
            return Forbid();

        var prescriptionItems = await _prescriptionItemService.GetAllByPrescriptionId(prescriptionId);
        if (prescriptionItems == null || !prescriptionItems.Any())
        {
            return NotFound("لا توجد أدوية لهذه الوصفة.");
        }
        return Ok(prescriptionItems);
    }

    // POST: api/prescriptionitems
    [Authorize(Policy = "AdminOrDoctorRole")]
    [HttpPost("CreatePrescriptionItem")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionItemDto>> CreatePrescriptionItem([FromBody] CreatePrescriptionItemDto prescriptionItemDto)
    {
        if (prescriptionItemDto == null)
        {
            return BadRequest("Invalid PrescriptionItem data.");
        }

        //var prescription = await _prescriptionService.GetByIdAsync(prescriptionItemDto.PrescriptionId);

        //if (prescription == null)
        //{
        //    return BadRequest("الوصفه مش موجودة");

        //}

       

    //    var auth = await _authorizationService.AuthorizeAsync(
    //User, prescription, "CanEditPrescription");

        //if (!auth.Succeeded)
        //    return Forbid();

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
    [Authorize(Policy = "AdminOrDoctorRole")]
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

        var prescription = await _prescriptionService.GetByIdAsync(existingPrescriptionItem.PrescriptionId);
        if (prescription == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, prescription, "CanEditPrescription");

        if (!auth.Succeeded)
            return Forbid();

        var updatedPrescriptionItem = await _prescriptionItemService.UpdateAsync(id, prescriptionItemDto);
        if (updatedPrescriptionItem == null)
        {
            return NotFound();
        }
        return Ok(updatedPrescriptionItem);
    }

    // DELETE: api/prescriptionitems/{id}
    [Authorize(Policy = "AdminOrDoctorRole")]
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


            var prescription = await _prescriptionService.GetByIdAsync(existingPrescriptionItem.PrescriptionId);
            if (prescription == null)
                return NotFound();

            var auth = await _authorizationService.AuthorizeAsync(
                User, prescription, "CanEditPrescription");

            if (!auth.Succeeded)
                return Forbid();

            var deleted = await _prescriptionItemService.DeleteAsync(id);


            if (!deleted)
                return NotFound($"PrescriptionItem with ID {id} not found. no rows deleted!");

            return Ok($"PrescriptionItem center ID {id} has been deleted.");

        }
        catch (KeyNotFoundException)
        {
            return NotFound($"PrescriptionItem with ID {id} not found. no rows deleted!");
        }
    }

    private async Task<(PrescriptionDto?, bool)> CheckAccess(int prescriptionId)
    {
        var prescription = await _prescriptionService.GetByIdAsync(prescriptionId);
        if (prescription == null) return (null, false);

        var auth = await _authorizationService.AuthorizeAsync(
            User, prescription, "CanAccessPrescription");

        return (prescription, auth.Succeeded);
    }
}