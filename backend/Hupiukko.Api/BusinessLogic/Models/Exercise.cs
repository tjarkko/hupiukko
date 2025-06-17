namespace Hupiukko.Api.BusinessLogic.Models;

public class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? MuscleGroups { get; set; }
    public string? Equipment { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ExerciseCategory Category { get; set; } = null!;
    public virtual ICollection<ProgramExercise> ProgramExercises { get; set; } = new List<ProgramExercise>();
} 