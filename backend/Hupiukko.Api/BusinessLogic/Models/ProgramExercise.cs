namespace Hupiukko.Api.BusinessLogic.Models;

public enum DayOfWeek
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}

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
    public DayOfWeek? DayOfWeek { get; set; } // 0=Sunday, 1=Monday, ..., 6=Saturday

    // Navigation properties
    public virtual WorkoutProgram Program { get; set; } = null!;
    public virtual Exercise Exercise { get; set; } = null!;
    public virtual ICollection<ProgramExerciseSet> ProgramExerciseSets { get; set; } = new List<ProgramExerciseSet>();
    public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
} 