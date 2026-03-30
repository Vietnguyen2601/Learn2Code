using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IDiscussionRepository : IGenericRepository<Discussion>
{
    /// <summary>
    /// L?y t?t c? discussion c?a m?t lesson
    /// </summary>
    Task<List<Discussion>> GetDiscussionsByLessonIdAsync(Guid lessonId);

    /// <summary>
    /// L?y discussion v?i chi ti?t (including creator, comments, ...)
    /// </summary>
    Task<Discussion?> GetDiscussionWithDetailsAsync(Guid discussionId);

    /// <summary>
    /// L?y discussion pinned c?a m?t lesson
    /// </summary>
    Task<List<Discussion>> GetPinnedDiscussionsByLessonIdAsync(Guid lessonId);
}
