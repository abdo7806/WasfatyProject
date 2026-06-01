using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IServices;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PharmacistController : ControllerBase
{
    private readonly IPharmacistService _pharmacistService;
    private readonly IUserService _userService;
    private readonly IPharmacyService _pharmacyService;
    private readonly IAuthorizationService _authorizationService;


    public PharmacistController(IPharmacistService pharmacistService,
        IUserService userService,
        IPharmacyService pharmacyService,
        IAuthorizationService authorizationService)
    {
        _pharmacistService = pharmacistService;
        _userService = userService;
        _pharmacyService = pharmacyService;
        _authorizationService = authorizationService;
    }

    [Authorize(Policy = "AdminRole")]
    [HttpGet("All", Name = "GetAllPharmacist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PharmacistDto>>> GetAllPharmacist()
    {
        var pharmacists = await _pharmacistService.GetAllAsync();
        if (pharmacists == null || !pharmacists.Any())
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
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, pharmacist, "CanAccessPharmacist");

        if (!auth.Succeeded)
            return Forbid();


        return Ok(pharmacist);
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost("CreatePharmacist")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacistDto>> CreatePharmacist([FromBody] CreatePharmacistDto pharmacistDto)
    {
        if (pharmacistDto == null)
        {
            return BadRequest("Invalid pharmacist data.");
        }


        PharmacyDto? pharmacy = await _pharmacyService.GetByIdAsync(pharmacistDto.PharmacyId);

        if (pharmacy == null)
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


    [Authorize(Policy = "AdminOrPharmacistRole")]
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


        var existingPharmacist = await _pharmacistService.GetByIdAsync(id);

        if (existingPharmacist == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, existingPharmacist, "CanEditPharmacist");

        if (!auth.Succeeded)
            return Forbid();

        var pharmacy = await _pharmacyService.GetByIdAsync(pharmacistDto.PharmacyId);

        if (pharmacy == null)
        {
            return BadRequest("Invalid Pharmacy data.");

        }

        var updatedPharmacist = await _pharmacistService.UpdateAsync(id, pharmacistDto);

        return Ok(updatedPharmacist);

 
    }

    [Authorize(Policy = "AdminRole")]
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
                return Ok($"Pharmacist ID {id} has been deleted.");
            }

            if (await _pharmacistService.DeleteAsync(id))
                return Ok($"Pharmacist center ID {id} has been deleted.");
            else
                return NotFound($"Pharmacist with ID {id} not found.");

        }
        catch (KeyNotFoundException)
        {
            return Ok($"Pharmacist ID {id} has been deleted.");
        }

    }

    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpGet("GetByPharmacyIdAsync/{PharmacyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PharmacistDto>>> GetByPharmacyIdAsync(int PharmacyId)
    {
        var pharmacists = await _pharmacistService.GetByPharmacyIdAsync(PharmacyId);


        if (pharmacists == null || !pharmacists.Any())
        {
            return NotFound("No pharmacists found.");
        }
        return Ok(pharmacists);
    }


    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpGet("GetPharmacistByUserId/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PharmacistDto>> GetPharmacistByUserId(int userId)
    {


        if (userId < 1)
        {
            return BadRequest("Invalid userId.");
        }

        var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(userId);

        if (pharmacist == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, pharmacist, "CanAccessPharmacist");

        if (!auth.Succeeded)
            return Forbid();

        return Ok(pharmacist);
    }


    [Authorize(Policy = "AdminOrPharmacistRole")]
    [HttpGet("stats/{pharmacistId}")]
    public async Task<IActionResult> GetDashboardStats(int pharmacistId)
    {
        var pharmacist = await _pharmacistService.GetByIdAsync(pharmacistId);

        if (pharmacist == null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(
            User, pharmacist, "CanAccessPharmacist");

        if (!auth.Succeeded)
            return Forbid();


        var stats = await _pharmacistService.GetPharmacistDataAsync(pharmacistId);
        return Ok(stats);
    }




}