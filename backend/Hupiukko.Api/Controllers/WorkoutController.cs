using Hupiukko.Api.BusinessLogic.Managers;
using Hupiukko.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hupiukko.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkoutController : BaseApiController
{
    private readonly IWorkoutManager _workoutManager;

    public WorkoutController(IWorkoutManager workoutManager)
    {
        _workoutManager = workoutManager;
    }

    #region Workout Programs

    /// <summary>
    /// Get current user's workout programs
    /// </summary>
    [HttpGet("programs")]
    public async Task<ActionResult<List<WorkoutProgramDto>>> GetUserPrograms()
    {
        if (CurrentUser == null) return Unauthorized();
        var programs = await _workoutManager.GetUserProgramsAsync(CurrentUser.Id);
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
    /// Create a new workout program for current user (with WorkoutDays)
    /// </summary>
    [HttpPost("programs")]
    public async Task<ActionResult<WorkoutProgramDto>> CreateProgram(CreateWorkoutProgramRequest request)
    {
        if (CurrentUser == null) return Unauthorized();
        var program = await _workoutManager.CreateProgramAsync(CurrentUser.Id, request);
        return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
    }

    #endregion

    #region Workout Sessions

    /// <summary>
    /// Get current user's workout sessions
    /// </summary>
    [HttpGet("sessions")]
    public async Task<ActionResult<List<WorkoutSessionDto>>> GetUserSessions()
    {
        if (CurrentUser == null) return Unauthorized();
        var sessions = await _workoutManager.GetUserWorkoutSessionsAsync(CurrentUser.Id);
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
    /// Get current user's active workout session
    /// </summary>
    [HttpGet("sessions/active")]
    public async Task<ActionResult<WorkoutSessionDto>> GetActiveSession()
    {
        if (CurrentUser == null) return Unauthorized();
        var session = await _workoutManager.GetActiveWorkoutSessionAsync(CurrentUser.Id);
        if (session == null)
            return NotFound("No active workout session found");
        return Ok(session);
    }

    /// <summary>
    /// Start a new workout session for current user (optionally for a specific WorkoutDay)
    /// </summary>
    [HttpPost("sessions/start")]
    public async Task<ActionResult<WorkoutSessionDto>> StartWorkout(StartWorkoutRequest request)
    {
        if (CurrentUser == null) return Unauthorized();
        try
        {
            var session = await _workoutManager.StartWorkoutAsync(CurrentUser.Id, request);
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

    // Example: Use CurrentUser to get weekly program
    // [HttpGet("weekly-program")]
    // public async Task<IActionResult> GetWeeklyProgram([FromServices] IWorkoutManager workoutManager)
    // {
    //     if (CurrentUser == null) return Unauthorized();
    //     var program = await workoutManager.GetWeeklyProgramForUser(CurrentUser.Id);
    //     return Ok(program);
    // }
} 