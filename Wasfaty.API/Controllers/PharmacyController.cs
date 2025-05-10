using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]
public class PharmacyController : ControllerBase
{
    private readonly IPharmacyService _pharmacyService;

    public PharmacyController(IPharmacyService pharmacyService)
    {
        _pharmacyService = pharmacyService;
    }

    // GET: api/pharmacy
    [HttpGet("All", Name = "GetAllPharmacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PharmacyDto>>> GetAllPharmacy()
    {
        var pharmacies = await _pharmacyService.GetAllAsync();
        if (pharmacies == null || !pharmacies.Any() || pharmacies.Count() == 0)
        {
            return NotFound("No pharmacies centers found.");
        }

        return Ok(pharmacies);
    }

    // GET: api/pharmacy/{id}
    [HttpGet("{id}", Name = "GetPharmacyById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacyDto>> GetPharmacyById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var pharmacy = await _pharmacyService.GetByIdAsync(id);
        if (pharmacy == null)
        {
            return NotFound($"Pharmacy center with ID {id} not found.");
        }
        return Ok(pharmacy);
    }

    // POST: api/pharmacy
    [HttpPost("CreatePharmacy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacyDto>> CreatePharmacy([FromBody] CreatePharmacyDto pharmacyDto)
    {
        if (pharmacyDto == null)
        {
            return BadRequest("Invalid pharmacy center data.");
        }


        var createdPharmacy = await _pharmacyService.CreateAsync(pharmacyDto);

        if (createdPharmacy == null)
        {
            return BadRequest("Not created");
        }

        return CreatedAtRoute("GetPharmacyById", new { id = createdPharmacy.Id }, createdPharmacy);
    }

    // PUT: api/pharmacy/{id}
    [HttpPut("{id}", Name = "UpdatePharmacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PharmacyDto>> UpdatePharmacy(int id, [FromBody] UpdatePharmacyDto pharmacyDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }
        var updatedPharmacy = await _pharmacyService.UpdateAsync(id, pharmacyDto);
        if (updatedPharmacy == null)
        {
            return NotFound($"Pharmacy center with ID {id} not found.");
        }
        return Ok(updatedPharmacy);
    }

    // DELETE: api/pharmacy/{id}
    [HttpDelete("{id}", Name = "DeletePharmacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePharmacy(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingPharmacy = await _pharmacyService.GetByIdAsync(id);
            if (existingPharmacy == null)
            {
                return NotFound($"Pharmacy center with ID {id} not found.");
            }

            if (await _pharmacyService.DeleteAsync(id))
                return Ok($"Pharmacy center ID {id} has been deleted.");
            else
                return NotFound($"Pharmacy with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Pharmacy with ID {id} not found. no rows deleted!");
        }

    }
}