using Hupiukko.Api.BusinessLogic.Managers;
using Hupiukko.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hupiukko.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExercisesController : BaseApiController
{
    private readonly IExercisesManager _exercisesManager;

    public ExercisesController(IExercisesManager exercisesManager)
    {
        _exercisesManager = exercisesManager;
    }

    /// <summary>
    /// Get all exercise categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<ExerciseCategoryDto>>> GetCategories()
    {
        var categories = await _exercisesManager.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get all exercises
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercises()
    {
        var exercises = await _exercisesManager.GetExercisesAsync();
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercises by category
    /// </summary>
    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercisesByCategory(Guid categoryId)
    {
        var exercises = await _exercisesManager.GetExercisesByCategoryAsync(categoryId);
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercise by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(Guid id)
    {
        var exercise = await _exercisesManager.GetExerciseByIdAsync(id);
        
        if (exercise == null)
            return NotFound();

        return Ok(exercise);
    }

    /// <summary>
    /// Create a new exercise
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise(CreateExerciseRequest request)
    {
        var exercise = await _exercisesManager.CreateExerciseAsync(request);
        return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
    }
} 