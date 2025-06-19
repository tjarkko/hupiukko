using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Hupiukko.Api.BusinessLogic.Db;
using Hupiukko.Api.BusinessLogic.Models;
using Hupiukko.Api.Dtos;
using Hupiukko.Api.BusinessLogic.Managers;
using Microsoft.Extensions.Logging;

public class UserProvisioningMiddleware
{
    private readonly RequestDelegate _next;

    public UserProvisioningMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db, IWorkoutManager workoutManager)
    {
        var logger = context.RequestServices.GetService(typeof(ILogger<UserProvisioningMiddleware>)) as ILogger<UserProvisioningMiddleware>;
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        logger?.LogInformation("UserProvisioningMiddleware invoked. Method: {Method}, Path: {Path}, RemoteIp: {RemoteIp}, AuthHeader: {AuthHeader}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            authHeader);
        if (context.User.Identity?.IsAuthenticated == true)
        {
            logger?.LogInformation("Claims: {Claims}", string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));
            var oid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(oid))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.AzureAdObjectId == oid);

                // Extract info from claims
                var email = context.User.FindFirst("email")?.Value ?? string.Empty;
                var name = context.User.FindFirst("name")?.Value;
                var firstName = context.User.FindFirst("given_name")?.Value;
                var lastName = context.User.FindFirst("family_name")?.Value;

                bool needsSave = false;
                bool userWasCreated = false;

                if (user == null)
                {
                    user = new User
                    {
                        AzureAdObjectId = oid,
                        Email = email,
                        DisplayName = name,
                        FirstName = firstName,
                        LastName = lastName,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                        // Set other defaults as needed
                    };
                    db.Users.Add(user);
                    needsSave = true;
                    userWasCreated = true;
                }
                else
                {
                    // Update user info if changed
                    if (user.Email != email) { user.Email = email; needsSave = true; }
                    if (user.DisplayName != name) { user.DisplayName = name; needsSave = true; }
                    if (user.FirstName != firstName) { user.FirstName = firstName; needsSave = true; }
                    if (user.LastName != lastName) { user.LastName = lastName; needsSave = true; }
                }

                if (needsSave)
                {
                    await db.SaveChangesAsync();
                }

                // TEMP: Create a default program for new users
                if (userWasCreated)
                {
                    // Fetch real exercise IDs from the DB by name
                    var benchPress = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Bench Press");
                    var squat = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Squat");
                    var deadlift = await db.Exercises.FirstOrDefaultAsync(e => e.Name == "Deadlift");

                    var defaultExercises = new List<CreateProgramExerciseRequest>();
                    int sortOrder = 1;
                    if (benchPress != null)
                    {
                        defaultExercises.Add(new CreateProgramExerciseRequest
                        {
                            ExerciseId = benchPress.Id,
                            SortOrder = sortOrder++,
                            TargetSets = 3,
                            DefaultRepsMin = 10,
                            DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Monday
                        });
                    }
                    if (squat != null)
                    {
                        defaultExercises.Add(new CreateProgramExerciseRequest
                        {
                            ExerciseId = squat.Id,
                            SortOrder = sortOrder++,
                            TargetSets = 4,
                            DefaultRepsMin = 8,
                            DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Wednesday
                        });
                    }
                    if (deadlift != null)
                    {
                        defaultExercises.Add(new CreateProgramExerciseRequest
                        {
                            ExerciseId = deadlift.Id,
                            SortOrder = sortOrder++,
                            TargetSets = 5,
                            DefaultRepsMin = 5,
                            DayOfWeek = Hupiukko.Api.BusinessLogic.Models.DayOfWeek.Friday
                        });
                    }

                    var defaultProgram = new CreateWorkoutProgramRequest
                    {
                        Name = "Starter Program",
                        Description = "A default program created for new users.",
                        IsActive = true,
                        StartDate = DateTime.UtcNow,
                        ProgramExercises = defaultExercises
                    };
                    await workoutManager.CreateProgramAsync(user.Id, defaultProgram);
                }

                // Attach user to HttpContext.Items for controller access
                context.Items["CurrentUser"] = user;
            }
        }
        else
        {
            logger?.LogWarning("User is not authenticated in middleware.");
        }
        await _next(context);
    }
} 