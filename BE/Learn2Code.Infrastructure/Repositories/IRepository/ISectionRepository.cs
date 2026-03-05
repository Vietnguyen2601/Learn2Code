using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ISectionRepository : IGenericRepository<Section>
{
    Task<List<Section>> GetByCourseIdAsync(Guid courseId);
    Task<Section?> GetByIdWithCourseAsync(Guid sectionId);
    Task<bool> HasDuplicateOrderInCourseAsync(Guid courseId, int orderNumber, Guid? excludeSectionId = null);
}
