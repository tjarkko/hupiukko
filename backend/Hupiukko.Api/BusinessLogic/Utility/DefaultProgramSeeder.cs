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
        var benchPress = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Bench Press");
        var squat = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Squat");
        var deadlift = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Deadlift");

        var workoutDays = new List<CreateWorkoutDayRequest>();
        int sortOrder = 1;
        if (benchPress != null)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Monday - Bench Press",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Monday,
                SortOrder = sortOrder++,
                ProgramExercises = new List<CreateProgramExerciseRequest>
                {
                    new CreateProgramExerciseRequest
                    {
                        ExerciseId = benchPress.Id,
                        SortOrder = 1,
                        TargetSets = 3,
                        DefaultRepsMin = 10
                    }
                }
            });
        }
        if (squat != null)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Wednesday - Squat",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Wednesday,
                SortOrder = sortOrder++,
                ProgramExercises = new List<CreateProgramExerciseRequest>
                {
                    new CreateProgramExerciseRequest
                    {
                        ExerciseId = squat.Id,
                        SortOrder = 1,
                        TargetSets = 4,
                        DefaultRepsMin = 8
                    }
                }
            });
        }
        if (deadlift != null)
        {
            workoutDays.Add(new CreateWorkoutDayRequest
            {
                Name = "Friday - Deadlift",
                DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Friday,
                SortOrder = sortOrder++,
                ProgramExercises = new List<CreateProgramExerciseRequest>
                {
                    new CreateProgramExerciseRequest
                    {
                        ExerciseId = deadlift.Id,
                        SortOrder = 1,
                        TargetSets = 5,
                        DefaultRepsMin = 5
                    }
                }
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