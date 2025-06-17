namespace Hupiukko.Api.BusinessLogic.Models;

public class ProgramSuggestion : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CurrentProgramId { get; set; }
    public string SuggestionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SuggestionData { get; set; }
    public string? Reasoning { get; set; }
    public string Status { get; set; } = "PENDING";

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual WorkoutProgram CurrentProgram { get; set; } = null!;
} 