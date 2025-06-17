namespace Hupiukko.Api.BusinessLogic.Models;

public class ProgramExerciseSet : BaseEntity
{
    public Guid ProgramExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int TargetRepsMin { get; set; }
    public int? TargetRepsMax { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? RestTimeSeconds { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ProgramExercise ProgramExercise { get; set; } = null!;
    public virtual ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
} 