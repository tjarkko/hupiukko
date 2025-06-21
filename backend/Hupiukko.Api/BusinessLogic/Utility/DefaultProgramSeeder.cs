using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hupiukko.Api.BusinessLogic.Db;
using Hupiukko.Api.BusinessLogic.Models;
using Hupiukko.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Hupiukko.Api.BusinessLogic.Managers;

namespace Hupiukko.Api.BusinessLogic.Utility;

public static class DefaultProgramSeeder
{
    public static async Task CreateDefaultProgramForUserAsync(User user, ApplicationDbContext db, IWorkoutManager workoutManager)
    {
        // Fetch real exercise IDs from the DB by name
        var pushUps = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Push-ups");
        var benchPress = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Bench Press");
        var squat = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Squats");
        var lunges = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Lunges");
        var legPress = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Leg Press");
        var pullups = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Pull-ups");
        var dumbbellRows = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Dumbbell Rows");
        var plank = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Plank");

        var workoutDays = new List<CreateWorkoutDayRequest>();
        int sortOrder = 1;
        // Day 1 - Push/Upper
        var day1Exercises = new List<CreateProgramExerciseRequest>();
        if (pushUps != null)
            day1Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = pushUps.Id, SortOrder = 1, TargetSets = 3, DefaultRepsMin = 10 });
        if (benchPress != null)
            day1Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = benchPress.Id, SortOrder = 2, TargetSets = 3, DefaultRepsMin = 8 });
        if (day1Exercises.Count > 0)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Day 1 - Push/Upper",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Monday,
                SortOrder = sortOrder++,
                ProgramExercises = day1Exercises
            });
        }
        // Day 2 - Legs
        var day2Exercises = new List<CreateProgramExerciseRequest>();
        if (squat != null)
            day2Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = squat.Id, SortOrder = 1, TargetSets = 4, DefaultRepsMin = 10 });
        if (lunges != null)
            day2Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = lunges.Id, SortOrder = 2, TargetSets = 3, DefaultRepsMin = 12 });
        if (legPress != null)
            day2Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = legPress.Id, SortOrder = 3, TargetSets = 3, DefaultRepsMin = 12 });
        if (day2Exercises.Count > 0)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Day 2 - Legs",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Wednesday,
                SortOrder = sortOrder++,
                ProgramExercises = day2Exercises
            });
        }
        // Day 3 - Pull/Core
        var day3Exercises = new List<CreateProgramExerciseRequest>();
        if (pullups != null)
            day3Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = pullups.Id, SortOrder = 1, TargetSets = 3, DefaultRepsMin = 8 });
        if (dumbbellRows != null)
            day3Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = dumbbellRows.Id, SortOrder = 2, TargetSets = 3, DefaultRepsMin = 10 });
        if (plank != null)
            day3Exercises.Add(new CreateProgramExerciseRequest { ExerciseId = plank.Id, SortOrder = 3, TargetSets = 3, DefaultRepsMin = 30 });
        if (day3Exercises.Count > 0)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Day 3 - Pull/Core",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Friday,
                SortOrder = sortOrder++,
                ProgramExercises = day3Exercises
            });
        }

        var defaultProgram = new CreateWorkoutProgramRequest
        {
            Name = "Starter Program",
            Description = "A default program created for new users.",
            IsActive = true,
            StartDate = DateTime.UtcNow,
            WorkoutDays = workoutDays
        };
        await workoutManager.CreateProgramAsync(user.Id, defaultProgram);
    }
} 