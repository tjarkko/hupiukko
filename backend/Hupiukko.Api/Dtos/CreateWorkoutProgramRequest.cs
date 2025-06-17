namespace Hupiukko.Api.Dtos;

using System;
using System.Collections.Generic;

public class CreateWorkoutProgramRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<CreateProgramExerciseRequest> ProgramExercises { get; set; } = new();
}

public class CreateProgramExerciseRequest
{
    public Guid ExerciseId { get; set; }
    public int SortOrder { get; set; }
    public int TargetSets { get; set; }
    public int DefaultRepsMin { get; set; }
    public int? DefaultRepsMax { get; set; }
    public decimal? DefaultWeight { get; set; }
    public int? DefaultRestTimeSeconds { get; set; }
    public string? Notes { get; set; }
    public List<CreateProgramExerciseSetRequest> ProgramExerciseSets { get; set; } = new();
}

public class CreateProgramExerciseSetRequest
{
    public int SetNumber { get; set; }
    public int TargetRepsMin { get; set; }
    public int? TargetRepsMax { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? RestTimeSeconds { get; set; }
    public string? Notes { get; set; }
} 