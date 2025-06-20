using System;
using System.Collections.Generic;

namespace Hupiukko.Api.BusinessLogic.Models;

public class WorkoutDay : BaseEntity
{
    public Guid ProgramId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DayOfWeek? DayOfWeek { get; set; } // Optional: for named weekdays
    public int SortOrder { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual WorkoutProgram Program { get; set; } = null!;
    public virtual ICollection<ProgramExercise> ProgramExercises { get; set; } = new List<ProgramExercise>();
} 