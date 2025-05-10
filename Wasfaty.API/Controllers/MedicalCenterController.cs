using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Wasfaty.Application.DTOs; // تأكد من تعديل المسار إذا لزم الأمر
using Wasfaty.Application.Interfaces;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Infrastructure.Services;
using Wasfaty.Application.Constants;

[Route("api/[controller]")]
[ApiController]
//[Authorize] // يتطلب تسجيل الدخول للوصول إلى جميع الدوال

[Authorize(Roles = Roles.Admin)]

public class MedicalCenterController : ControllerBase
{
    private readonly IMedicalCenterService _medicalCenterService;

    public MedicalCenterController(IMedicalCenterService medicalCenterService)
    {
        _medicalCenterService = medicalCenterService;
    }

    // GET: api/medicalcenter
    [HttpGet("All", Name = "GetAllMedicalCenters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MedicalCenterDto>>> GetAllMedicalCenters()
    {
        var medicalCenters = await _medicalCenterService.GetAllAsync();

        if (medicalCenters == null || !medicalCenters.Any() || medicalCenters.Count() == 0)
        {
            return NotFound("No medical centers found.");
        }
        return Ok(medicalCenters);
    }

    // GET: api/medicalcenter/{id}
    [HttpGet("{id}", Name = "GetMedicalCenterById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicalCenterDto>> GetMedicalCenterById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var medicalCenter = await _medicalCenterService.GetByIdAsync(id);
        if (medicalCenter == null)
        {
            return NotFound($"Medical center with ID {id} not found.");
        }
        return Ok(medicalCenter);
    }

    // POST: api/medicalcenter
    [HttpPost("CreateMedicalCenter")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMedicalCenter([FromBody] CreateMedicalCenterDto medicalCenterDto)
    {
        if (medicalCenterDto == null)
        {
            return BadRequest("Invalid medical center data.");
        }
 


        var medicalCenter = await _medicalCenterService.CreateAsync(medicalCenterDto);

        if (medicalCenter == null)
        {
            return BadRequest("Not created");
        }

        return CreatedAtRoute("GetMedicalCenterById", new { id = medicalCenter.Id }, medicalCenter);
    }

    // PUT: api/medicalcenter/{id}
    [HttpPut("{id}", Name = "UpdateMedicalCenter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMedicalCenter(int id, [FromBody] UpdateMedicalCenterDto medicalCenterDto)
    {
        if (id < 1)
        {
            return BadRequest("Invalid ID.");
        }

        var existingMedicalCenter = await _medicalCenterService.GetByIdAsync(id);
        if (existingMedicalCenter == null)
        {
            return NotFound($"Medical center with ID {id} not found.");
        }

        await _medicalCenterService.UpdateAsync(id, medicalCenterDto);
        return Ok(medicalCenterDto);
    }

    // DELETE: api/medicalcenter/{id}
    [HttpDelete("{id}", Name = "DeleteMedicalCenter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteMedicalCenter(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest("Invalid ID.");
            }

            var existingMedicalCenter = await _medicalCenterService.GetByIdAsync(id);
            if (existingMedicalCenter == null)
            {
                return NotFound($"Medical center with ID {id} not found.");
            }

            if (await _medicalCenterService.DeleteAsync(id))
                return Ok($"Medical center ID {id} has been deleted.");
            else
                return NotFound($"Medical with ID {id} not found. no rows deleted!");
     

        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Medical with ID {id} not found. no rows deleted!");
        }
    }
}