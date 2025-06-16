using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // يتطلب تسجيل الدخول للوصول إلى جميع الدوال في هذه الوحدة
  //  [Authorize(Roles = Roles.Admin)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        // GET: api/user
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            if (users.Count() == 0)
            {
                return NotFound("No Users Found!");
            }
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
      //  [Authorize(Roles = Roles.Admin)]

        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest($"Not accepted ID {id}");
                }
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found.");
            }
        }

        // POST: api/user
        [HttpPost(Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
       // [Authorize(Roles = Roles.Admin)]

        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.FullName) || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("Invalid User data.");
            }
            UserDto user = await _userService.CreateUserAsync(userDto);
            if (user == null)
            {
                return BadRequest("اسم المستخدم موجود بالفعل");
            }
            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);


        }

        // PUT: api/user/{id}
        [Authorize(Roles = Roles.Admin + "," + Roles.Patient)] // استثناء

        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       // [Authorize(Roles = Roles.Admin)]

        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {

            if (userDto == null || string.IsNullOrEmpty(userDto.FullName) || string.IsNullOrEmpty(userDto.Email) )
            {
                return BadRequest("Invalid User data.");
            }


            try
            {
                // await _userService.UpdateUserAsync(userDto);

                UserDto user = await _userService.UpdateUserAsync(id, userDto);
                if (user == null)
                {
                    return BadRequest("The user has not been modified");
                }
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("The user has not been modified");
            }
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       // [Authorize(Roles = Roles.Admin)]

        public async Task<ActionResult> DeleteUser(int id)
        {
        
            try
            {
                if (id < 1)
                {
                    return BadRequest($"Not accepted ID {id}");
                }

                if(await _userService.DeleteUserAsync(id))
                    return Ok($"User with ID {id} has been deleted.");
                else
                    return NotFound($"User with ID {id} not found. no rows deleted!");


     
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found. no rows deleted!");
            }
        }
    }
}
