namespace Hupiukko.Api.Dtos;

using System;
using System.Collections.Generic;

public class WorkoutSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public int? DurationMinutes { get; set; }
    public List<WorkoutExerciseDto> WorkoutExercises { get; set; } = new();
}

public class WorkoutExerciseDto
{
    public Guid Id { get; set; }
    public Guid ProgramExerciseId { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
    public List<WorkoutSetDto> WorkoutSets { get; set; } = new();
} 