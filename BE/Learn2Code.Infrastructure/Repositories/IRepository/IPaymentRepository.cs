using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    /// <summary>
    /// Get all payments for a specific student with subscription details
    /// </summary>
    Task<List<Payment>> GetByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Get all payments with subscription and user details (for admin)
    /// </summary>
    Task<List<Payment>> GetAllWithDetailsAsync();
}
