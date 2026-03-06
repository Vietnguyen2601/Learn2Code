using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Course>> GetAllWithRelationsAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null)
    {
        var query = _context.Set<Course>()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.IsActive);

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        // Filter by difficulty
        if (difficulty.HasValue)
        {
            query = query.Where(c => c.Difficulty == difficulty.Value);
        }

        // Search by title or description
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(c => 
                c.Title.ToLower().Contains(searchLower) || 
                (c.Description != null && c.Description.ToLower().Contains(searchLower))
            );
        }

        return await query
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Course>> GetAllWithRelationsByStatusAsync(
        bool isActive,
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null)
    {
        var query = _context.Set<Course>()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.IsActive == isActive);

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        // Filter by difficulty
        if (difficulty.HasValue)
        {
            query = query.Where(c => c.Difficulty == difficulty.Value);
        }

        // Search by title or description
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(c => 
                c.Title.ToLower().Contains(searchLower) || 
                (c.Description != null && c.Description.ToLower().Contains(searchLower))
            );
        }

        return await query
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdWithRelationsAsync(Guid id)
    {
        return await _context.Set<Course>()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Sections.Where(s => s.IsActive).OrderBy(s => s.OrderNumber))
            .FirstOrDefaultAsync(c => c.CourseId == id);
    }
}
