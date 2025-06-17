namespace Hupiukko.Api.BusinessLogic.Models;

public class WorkoutExercise : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid ProgramExerciseId { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual WorkoutSession Session { get; set; } = null!;
    public virtual ProgramExercise ProgramExercise { get; set; } = null!;
    public virtual ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
} 