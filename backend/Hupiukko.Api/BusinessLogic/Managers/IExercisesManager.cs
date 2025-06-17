using Hupiukko.Api.Dtos;

namespace Hupiukko.Api.BusinessLogic.Managers;

public interface IExercisesManager
{
    Task<List<ExerciseCategoryDto>> GetCategoriesAsync();
    Task<List<ExerciseDto>> GetExercisesAsync();
    Task<List<ExerciseDto>> GetExercisesByCategoryAsync(Guid categoryId);
    Task<ExerciseDto?> GetExerciseByIdAsync(Guid id);
    Task<ExerciseDto> CreateExerciseAsync(CreateExerciseRequest request);
} 