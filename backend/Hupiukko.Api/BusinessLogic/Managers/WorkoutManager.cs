using AutoMapper;
using Hupiukko.Api.BusinessLogic.Db;
using Hupiukko.Api.BusinessLogic.Models;
using Hupiukko.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hupiukko.Api.BusinessLogic.Managers;

public class WorkoutManager : IWorkoutManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WorkoutManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WorkoutProgramDto>> GetUserProgramsAsync(Guid userId)
    {
        var programs = await _context.WorkoutPrograms
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.Exercise)
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.ProgramExerciseSets)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.IsActive)
            .ThenByDescending(p => p.StartDate)
            .ToListAsync();

        return _mapper.Map<List<WorkoutProgramDto>>(programs);
    }

    public async Task<WorkoutProgramDto?> GetProgramByIdAsync(Guid id)
    {
        var program = await _context.WorkoutPrograms
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.Exercise)
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.ProgramExerciseSets)
            .FirstOrDefaultAsync(p => p.Id == id);

        return program != null ? _mapper.Map<WorkoutProgramDto>(program) : null;
    }

    public async Task<WorkoutProgramDto> CreateProgramAsync(Guid userId, CreateWorkoutProgramRequest request)
    {
        var program = _mapper.Map<WorkoutProgram>(request);
        program.UserId = userId;

        // Set current active program to inactive if this is active
        if (program.IsActive)
        {
            var currentActive = await _context.WorkoutPrograms
                .Where(p => p.UserId == userId && p.IsActive)
                .ToListAsync();

            foreach (var activeProgram in currentActive)
            {
                activeProgram.IsActive = false;
            }
        }

        _context.WorkoutPrograms.Add(program);
        await _context.SaveChangesAsync();

        // Reload with includes for mapping
        await _context.Entry(program)
            .Collection(p => p.ProgramExercises)
            .Query()
            .Include(pe => pe.Exercise)
            .Include(pe => pe.ProgramExerciseSets)
            .LoadAsync();

        return _mapper.Map<WorkoutProgramDto>(program);
    }

    public async Task<List<WorkoutSessionDto>> GetUserWorkoutSessionsAsync(Guid userId)
    {
        var sessions = await _context.WorkoutSessions
            .Include(s => s.Program)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.ProgramExercise)
                    .ThenInclude(pe => pe.Exercise)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.WorkoutSets)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        return _mapper.Map<List<WorkoutSessionDto>>(sessions);
    }

    public async Task<WorkoutSessionDto?> GetWorkoutSessionByIdAsync(Guid id)
    {
        var session = await _context.WorkoutSessions
            .Include(s => s.Program)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.ProgramExercise)
                    .ThenInclude(pe => pe.Exercise)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.WorkoutSets)
            .FirstOrDefaultAsync(s => s.Id == id);

        return session != null ? _mapper.Map<WorkoutSessionDto>(session) : null;
    }

    public async Task<WorkoutSessionDto> StartWorkoutAsync(Guid userId, StartWorkoutRequest request)
    {
        // Check for existing active session
        var activeSession = await _context.WorkoutSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsCompleted);

        if (activeSession != null)
            throw new InvalidOperationException("Cannot start workout: active session already exists");

        // Get the program with exercises
        var program = await _context.WorkoutPrograms
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.Exercise)
            .Include(p => p.ProgramExercises)
                .ThenInclude(pe => pe.ProgramExerciseSets)
            .FirstOrDefaultAsync(p => p.Id == request.ProgramId && p.UserId == userId);

        if (program == null)
            throw new ArgumentException("Program not found or not accessible");

        // Create workout session
        var session = new WorkoutSession
        {
            UserId = userId,
            ProgramId = request.ProgramId,
            StartTime = DateTime.UtcNow
        };

        _context.WorkoutSessions.Add(session);

        // Create workout exercises based on program
        foreach (var programExercise in program.ProgramExercises.OrderBy(pe => pe.SortOrder))
        {
            var workoutExercise = new WorkoutExercise
            {
                SessionId = session.Id,
                ProgramExerciseId = programExercise.Id,
                SortOrder = programExercise.SortOrder
            };

            _context.WorkoutExercises.Add(workoutExercise);

            // Create workout sets based on program exercise sets or defaults
            var sets = programExercise.ProgramExerciseSets.Any() 
                ? programExercise.ProgramExerciseSets.OrderBy(s => s.SetNumber).ToList()
                : null;

            if (sets != null && sets.Any())
            {
                // Create sets based on program exercise sets
                foreach (var programSet in sets)
                {
                    var workoutSet = new WorkoutSet
                    {
                        WorkoutExerciseId = workoutExercise.Id,
                        ProgramExerciseSetId = programSet.Id,
                        SetNumber = programSet.SetNumber,
                        Reps = programSet.TargetRepsMin, // Default to min reps
                        Weight = programSet.TargetWeight
                    };

                    _context.WorkoutSets.Add(workoutSet);
                }
            }
            else
            {
                // Create sets based on default program exercise settings
                for (int i = 1; i <= programExercise.TargetSets; i++)
                {
                    var workoutSet = new WorkoutSet
                    {
                        WorkoutExerciseId = workoutExercise.Id,
                        SetNumber = i,
                        Reps = programExercise.DefaultRepsMin,
                        Weight = programExercise.DefaultWeight
                    };

                    _context.WorkoutSets.Add(workoutSet);
                }
            }
        }

        await _context.SaveChangesAsync();

        return await GetWorkoutSessionByIdAsync(session.Id) ?? 
               throw new InvalidOperationException("Failed to retrieve created session");
    }

    public async Task<WorkoutSessionDto?> CompleteWorkoutAsync(Guid sessionId)
    {
        var session = await _context.WorkoutSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return null;

        session.EndTime = DateTime.UtcNow;
        session.IsCompleted = true;

        await _context.SaveChangesAsync();

        return await GetWorkoutSessionByIdAsync(sessionId);
    }

    public async Task<WorkoutSessionDto?> GetActiveWorkoutSessionAsync(Guid userId)
    {
        var session = await _context.WorkoutSessions
            .Include(s => s.Program)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.ProgramExercise)
                    .ThenInclude(pe => pe.Exercise)
            .Include(s => s.WorkoutExercises)
                .ThenInclude(we => we.WorkoutSets)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsCompleted);

        return session != null ? _mapper.Map<WorkoutSessionDto>(session) : null;
    }

    public async Task<WorkoutSetDto> CompleteSetAsync(Guid setId, CompleteSetRequest request)
    {
        var set = await _context.WorkoutSets
            .FirstOrDefaultAsync(s => s.Id == setId);

        if (set == null)
            throw new ArgumentException("Workout set not found");

        set.Reps = request.Reps;
        set.Weight = request.Weight;
        set.RestTimeSeconds = request.RestTimeSeconds;
        set.Notes = request.Notes;
        set.IsCompleted = true;
        set.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutSetDto>(set);
    }
} 