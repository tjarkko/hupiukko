namespace Hupiukko.Api.Dtos;

using System;
using System.Collections.Generic;

public class WorkoutProgramDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ProgramExerciseDto> ProgramExercises { get; set; } = new();
    public List<WorkoutSessionDto> WorkoutSessions { get; set; } = new();
    public List<ProgramSuggestionDto> ProgramSuggestions { get; set; } = new();
}

// Minimal ProgramExerciseDto for inclusion
public class ProgramExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public int SortOrder { get; set; }
    public int TargetSets { get; set; }
    public int DefaultRepsMin { get; set; }
    public int? DefaultRepsMax { get; set; }
    public decimal? DefaultWeight { get; set; }
    public int? DefaultRestTimeSeconds { get; set; }
    public string? Notes { get; set; }
    public List<ProgramExerciseSetDto> ProgramExerciseSets { get; set; } = new();
}

public class ProgramExerciseSetDto
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public int TargetRepsMin { get; set; }
    public int? TargetRepsMax { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? RestTimeSeconds { get; set; }
    public string? Notes { get; set; }
}

public class ProgramSuggestionDto
{
    public Guid Id { get; set; }
    public string SuggestionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SuggestionData { get; set; }
    public string? Reasoning { get; set; }
    public string Status { get; set; } = "PENDING";
} 