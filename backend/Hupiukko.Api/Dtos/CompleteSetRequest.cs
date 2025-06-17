namespace Hupiukko.Api.Dtos;

using System;

public class CompleteSetRequest
{
    public int Reps { get; set; }
    public decimal? Weight { get; set; }
    public int? RestTimeSeconds { get; set; }
    public string? Notes { get; set; }
} 