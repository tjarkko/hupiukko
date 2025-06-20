namespace Hupiukko.Api.BusinessLogic.Models;

public class WorkoutProgram : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
    public virtual ICollection<ProgramSuggestion> ProgramSuggestions { get; set; } = new List<ProgramSuggestion>();
    public virtual ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
} 