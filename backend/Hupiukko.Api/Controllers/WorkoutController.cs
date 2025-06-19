using Hupiukko.Api.BusinessLogic.Managers;
using Hupiukko.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hupiukko.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutManager _workoutManager;

    public WorkoutController(IWorkoutManager workoutManager)
    {
        _workoutManager = workoutManager;
    }

    #region Workout Programs

    /// <summary>
    /// Get user's workout programs
    /// </summary>
    [HttpGet("programs/user/{userId}")]
    public async Task<ActionResult<List<WorkoutProgramDto>>> GetUserPrograms(Guid userId)
    {
        var programs = await _workoutManager.GetUserProgramsAsync(userId);
        return Ok(programs);
    }

    /// <summary>
    /// Get workout program by ID
    /// </summary>
    [HttpGet("programs/{id}")]
    public async Task<ActionResult<WorkoutProgramDto>> GetProgram(Guid id)
    {
        var program = await _workoutManager.GetProgramByIdAsync(id);
        
        if (program == null)
            return NotFound();

        return Ok(program);
    }

    /// <summary>
    /// Create a new workout program
    /// </summary>
    [HttpPost("programs/user/{userId}")]
    public async Task<ActionResult<WorkoutProgramDto>> CreateProgram(Guid userId, CreateWorkoutProgramRequest request)
    {
        var program = await _workoutManager.CreateProgramAsync(userId, request);
        return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
    }

    #endregion

    #region Workout Sessions

    /// <summary>
    /// Get user's workout sessions
    /// </summary>
    [HttpGet("sessions/user/{userId}")]
    public async Task<ActionResult<List<WorkoutSessionDto>>> GetUserSessions(Guid userId)
    {
        var sessions = await _workoutManager.GetUserWorkoutSessionsAsync(userId);
        return Ok(sessions);
    }

    /// <summary>
    /// Get workout session by ID
    /// </summary>
    [HttpGet("sessions/{id}")]
    public async Task<ActionResult<WorkoutSessionDto>> GetSession(Guid id)
    {
        var session = await _workoutManager.GetWorkoutSessionByIdAsync(id);
        
        if (session == null)
            return NotFound();

        return Ok(session);
    }

    /// <summary>
    /// Get user's active workout session
    /// </summary>
    [HttpGet("sessions/user/{userId}/active")]
    public async Task<ActionResult<WorkoutSessionDto>> GetActiveSession(Guid userId)
    {
        var session = await _workoutManager.GetActiveWorkoutSessionAsync(userId);
        
        if (session == null)
            return NotFound("No active workout session found");

        return Ok(session);
    }

    /// <summary>
    /// Start a new workout session
    /// </summary>
    [HttpPost("sessions/user/{userId}/start")]
    public async Task<ActionResult<WorkoutSessionDto>> StartWorkout(Guid userId, StartWorkoutRequest request)
    {
        try
        {
            var session = await _workoutManager.StartWorkoutAsync(userId, request);
            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Complete a workout session
    /// </summary>
    [HttpPost("sessions/{sessionId}/complete")]
    public async Task<ActionResult<WorkoutSessionDto>> CompleteWorkout(Guid sessionId)
    {
        var session = await _workoutManager.CompleteWorkoutAsync(sessionId);
        
        if (session == null)
            return NotFound();

        return Ok(session);
    }

    #endregion

    #region Workout Sets

    /// <summary>
    /// Complete a workout set
    /// </summary>
    [HttpPost("sets/{setId}/complete")]
    public async Task<ActionResult<WorkoutSetDto>> CompleteSet(Guid setId, CompleteSetRequest request)
    {
        try
        {
            var set = await _workoutManager.CompleteSetAsync(setId, request);
            return Ok(set);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
} 