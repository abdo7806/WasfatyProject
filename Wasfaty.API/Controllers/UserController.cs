using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(IUserService userService,
                              IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

       

        // GET: api/user
        [Authorize(Policy = "AdminRole")]
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


                var auth = await _authorizationService.AuthorizeAsync(
                    User, user, "CanAccessUser");

                if (!auth.Succeeded)
                    return Forbid();

                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found.");
            }
        }

        // POST: api/user
        [Authorize(Policy = "AdminRole")]
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

                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                    return NotFound();

                var auth = await _authorizationService.AuthorizeAsync(
                    User, existingUser, "CanEditUser");

                if (!auth.Succeeded)
                    return Forbid();

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
        [Authorize(Policy = "AdminRole")]
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


                var exists = await _userService.GetUserByIdAsync(id);
                if (exists == null)
                    return NotFound();

                var deleted = await _userService.DeleteUserAsync(id);

                if (!deleted)
                    return NotFound($"User with ID {id} not found. no rows deleted!");

                return Ok($"User with ID {id} has been deleted.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found. no rows deleted!");
            }
        }
    }
}
