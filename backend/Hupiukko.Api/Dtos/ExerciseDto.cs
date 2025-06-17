namespace Hupiukko.Api.Dtos;

public class ExerciseCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? MuscleGroups { get; set; }
    public string? Equipment { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsActive { get; set; }
}

public class CreateExerciseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? MuscleGroups { get; set; }
    public string? Equipment { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
} 