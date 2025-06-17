using AutoMapper;
using Hupiukko.Api.BusinessLogic.Models;
using Hupiukko.Api.Dtos;

namespace Hupiukko.Api.BusinessLogic.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // WorkoutProgram <-> WorkoutProgramDto
        CreateMap<WorkoutProgram, WorkoutProgramDto>();
        CreateMap<ProgramExercise, ProgramExerciseDto>();
        CreateMap<ProgramExerciseSet, ProgramExerciseSetDto>();
        CreateMap<ProgramSuggestion, ProgramSuggestionDto>();
        CreateMap<WorkoutSession, WorkoutSessionDto>();
        CreateMap<WorkoutExercise, WorkoutExerciseDto>();
        CreateMap<WorkoutSet, WorkoutSetDto>();

        // Create requests to entities
        CreateMap<CreateWorkoutProgramRequest, WorkoutProgram>();
        CreateMap<CreateProgramExerciseRequest, ProgramExercise>();
        CreateMap<CreateProgramExerciseSetRequest, ProgramExerciseSet>();

        // Add missing mappings for exercises
        CreateMap<ExerciseCategory, ExerciseCategoryDto>();
        CreateMap<Exercise, ExerciseDto>();
        CreateMap<User, UserDto>();
    }
} 