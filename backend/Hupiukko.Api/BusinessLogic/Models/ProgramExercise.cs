namespace Hupiukko.Api.BusinessLogic.Models;

public class ProgramExercise : BaseEntity
{
    public Guid ProgramId { get; set; }
    public Guid ExerciseId { get; set; }
    public int SortOrder { get; set; }
    public int TargetSets { get; set; }
    public int DefaultRepsMin { get; set; }
    public int? DefaultRepsMax { get; set; }
    public decimal? DefaultWeight { get; set; }
    public int? DefaultRestTimeSeconds { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual WorkoutProgram Program { get; set; } = null!;
    public virtual Exercise Exercise { get; set; } = null!;
    public virtual ICollection<ProgramExerciseSet> ProgramExerciseSets { get; set; } = new List<ProgramExerciseSet>();
    public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
} 