using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]
public class PharmacistController : ControllerBase
{
    private readonly IPharmacistService _pharmacistService;
    private readonly IUserService _userService;
    private readonly IPharmacyService _pharmacyService;

    public PharmacistController(IPharmacistService pharmacistService,
        IUserService userService,
        IPharmacyService pharmacyService)
    {
        _pharmacistService = pharmacistService;
        _userService = userService;
        _pharmacyService = pharmacyService;
    }

    [HttpGet("All", Name = "GetAllPharmacist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PharmacistDto>>> GetAllPharmacist()
    {
        var pharmacists = await _pharmacistService.GetAllAsync();
        if (pharmacists == null || !pharmacists.Any() || pharmacists.Count() == 0)
        {
            return NotFound("No pharmacists found.");
        }
        return Ok(pharmacists);
    }

    [HttpGet("{id}", Name = "GetPharmacistById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacistDto>> GetPharmacistById(int id)
    {


        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var pharmacist = await _pharmacistService.GetByIdAsync(id);
        if (pharmacist == null)
        {
            return NotFound($"pharmacist with ID {id} not found.");
        }
        return Ok(pharmacist);
    }

    [HttpPost("CreatePharmacist")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacistDto>> CreatePharmacist([FromBody] CreatePharmacistDto pharmacistDto)
    {
        if (pharmacistDto == null)
        {
            return BadRequest("Invalid pharmacist data.");
        }


        var pharmacists = await _pharmacistService.GetAllAsync();

        if (pharmacists.Where(d => d.UserId == pharmacistDto.UserId).Count() > 0)
        {
            return BadRequest("هاذا المستخدم صيدلي بالفعل");

        }

        UserDto user = await _userService.GetUserByIdAsync(pharmacistDto.UserId);
        if (user == null)
        {
            return BadRequest("Invalid User data.");
        }

        if (user.Role != UserRoleEnum.Pharmacist)
        {
            return BadRequest("لازم تكون صلاحيات المستخدم صيدلي");

        }

        PharmacyDto? PharmacyDto = await _pharmacyService.GetByIdAsync(pharmacistDto.PharmacyId);

        if (PharmacyDto == null)
        {
            return BadRequest("Invalid Pharmacy data.");

        }

        var createdPharmacist = await _pharmacistService.CreateAsync(pharmacistDto);

        if (createdPharmacist == null)
        {
            return BadRequest("Not created");
        }
        return CreatedAtRoute("GetPharmacistById", new { id = createdPharmacist.Id }, createdPharmacist);
    }


    [HttpPut("{id}", Name = "UpdatePharmacist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PharmacistDto>> UpdatePharmacist(int id, [FromBody] UpdatePharmacistDto pharmacistDto)
    {

        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

       PharmacyDto? PharmacyDto = await _pharmacyService.GetByIdAsync(pharmacistDto.PharmacyId);

        if (PharmacyDto == null)
        {
            return BadRequest("Invalid Pharmacy data.");

        }

        var existingPharmacist = await _pharmacistService.GetByIdAsync(id);
        if (existingPharmacist == null)
        {
            return NotFound($"Pharmacy with ID {id} not found.");
        }

        var updatedPharmacist = await _pharmacistService.UpdateAsync(id, pharmacistDto);

        return Ok(updatedPharmacist);

 
    }

    [HttpDelete("{id}", Name = "DeletePharmacist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePharmacist(int id)
    {

        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingPharmacist = await _pharmacistService.GetByIdAsync(id);
            if (existingPharmacist == null)
            {
                return NotFound($"Pharmacist center with ID {id} not found.");
            }

            if (await _pharmacistService.DeleteAsync(id))
                return Ok($"Pharmacist center ID {id} has been deleted.");
            else
                return NotFound($"Pharmacist with ID {id} not found. no rows deleted!");


        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Pharmacist with ID {id} not found. no rows deleted!");
        }

    }


    [HttpGet("GetByPharmacyIdAsync/{PharmacyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PharmacistDto>>> GetByPharmacyIdAsync(int PharmacyId)
    {
        var pharmacists = await _pharmacistService.GetByPharmacyIdAsync(PharmacyId);
        if (pharmacists == null || !pharmacists.Any() || pharmacists.Count() == 0)
        {
            return NotFound("No pharmacists found.");
        }
        return Ok(pharmacists);
    }
}