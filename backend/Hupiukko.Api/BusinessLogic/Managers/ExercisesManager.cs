using AutoMapper;
using Hupiukko.Api.BusinessLogic.Db;
using Hupiukko.Api.BusinessLogic.Models;
using Hupiukko.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hupiukko.Api.BusinessLogic.Managers;

public class ExercisesManager : IExercisesManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ExercisesManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ExerciseCategoryDto>> GetCategoriesAsync()
    {
        var categories = await _context.ExerciseCategories
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return _mapper.Map<List<ExerciseCategoryDto>>(categories);
    }

    public async Task<List<ExerciseDto>> GetExercisesAsync()
    {
        var exercises = await _context.Exercises
            .Include(e => e.Category)
            .Where(e => e.IsActive)
            .OrderBy(e => e.Category.SortOrder)
            .ThenBy(e => e.Name)
            .ToListAsync();

        return _mapper.Map<List<ExerciseDto>>(exercises);
    }

    public async Task<List<ExerciseDto>> GetExercisesByCategoryAsync(Guid categoryId)
    {
        var exercises = await _context.Exercises
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();

        return _mapper.Map<List<ExerciseDto>>(exercises);
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid id)
    {
        var exercise = await _context.Exercises
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        return exercise != null ? _mapper.Map<ExerciseDto>(exercise) : null;
    }

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseRequest request)
    {
        var exercise = _mapper.Map<Exercise>(request);

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Reload with category for mapping
        await _context.Entry(exercise)
            .Reference(e => e.Category)
            .LoadAsync();

        return _mapper.Map<ExerciseDto>(exercise);
    }
} 