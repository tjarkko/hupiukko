using Hupiukko.Api.Dtos;

namespace Hupiukko.Api.BusinessLogic.Managers;

public interface IWorkoutManager
{
    // Workout Programs
    Task<List<WorkoutProgramDto>> GetUserProgramsAsync(Guid userId);
    Task<WorkoutProgramDto?> GetProgramByIdAsync(Guid id);
    Task<WorkoutProgramDto> CreateProgramAsync(Guid userId, CreateWorkoutProgramRequest request);

    // Workout Sessions
    Task<List<WorkoutSessionDto>> GetUserWorkoutSessionsAsync(Guid userId);
    Task<WorkoutSessionDto?> GetWorkoutSessionByIdAsync(Guid id);
    Task<WorkoutSessionDto> StartWorkoutAsync(Guid userId, StartWorkoutRequest request);
    Task<WorkoutSessionDto?> CompleteWorkoutAsync(Guid sessionId);
    Task<WorkoutSessionDto?> GetActiveWorkoutSessionAsync(Guid userId);

    // Workout Sets
    Task<WorkoutSetDto> CompleteSetAsync(Guid setId, CompleteSetRequest request);

    // Add signatures for WorkoutDay CRUD if needed
    // Task<WorkoutDayDto> CreateWorkoutDayAsync(...);
    // Task<List<WorkoutDayDto>> GetWorkoutDaysForProgramAsync(Guid programId);
} 