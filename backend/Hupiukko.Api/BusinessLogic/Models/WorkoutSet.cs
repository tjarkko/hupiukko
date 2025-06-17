namespace Hupiukko.Api.BusinessLogic.Models;

public class WorkoutSet : BaseEntity
{
    public Guid WorkoutExerciseId { get; set; }
    public Guid? ProgramExerciseSetId { get; set; }
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public decimal? Weight { get; set; }
    public int? RestTimeSeconds { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual WorkoutExercise WorkoutExercise { get; set; } = null!;
    public virtual ProgramExerciseSet? ProgramExerciseSet { get; set; }
} 