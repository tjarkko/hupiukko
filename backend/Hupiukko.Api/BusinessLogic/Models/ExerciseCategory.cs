namespace Hupiukko.Api.BusinessLogic.Models;

public class ExerciseCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
} 