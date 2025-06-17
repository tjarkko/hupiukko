namespace Hupiukko.Api.BusinessLogic.Models;

public class WorkoutSession : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }

    // Computed property for duration
    public int? DurationMinutes => EndTime.HasValue ? 
        (int)(EndTime.Value - StartTime).TotalMinutes : null;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual WorkoutProgram Program { get; set; } = null!;
    public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
} 