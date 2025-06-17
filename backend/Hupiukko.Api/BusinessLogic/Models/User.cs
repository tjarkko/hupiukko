namespace Hupiukko.Api.BusinessLogic.Models;

public class User : BaseEntity
{
    public string AzureAdObjectId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string TimeZone { get; set; } = "UTC";

    // Navigation properties
    public virtual ICollection<WorkoutProgram> WorkoutPrograms { get; set; } = new List<WorkoutProgram>();
    public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
    public virtual ICollection<ProgramSuggestion> ProgramSuggestions { get; set; } = new List<ProgramSuggestion>();
} 