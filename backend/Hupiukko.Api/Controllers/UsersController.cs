using Hupiukko.Api.BusinessLogic.Managers;
using Hupiukko.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hupiukko.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersManager _usersManager;

    public UsersController(IUsersManager usersManager)
    {
        _usersManager = usersManager;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _usersManager.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _usersManager.GetUserByIdAsync(id);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _usersManager.GetUserByEmailAsync(email);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
    {
        var user = await _usersManager.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, CreateUserRequest request)
    {
        var user = await _usersManager.UpdateUserAsync(id, request);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var success = await _usersManager.DeleteUserAsync(id);
        
        if (!success)
            return NotFound();

        return NoContent();
    }
} 