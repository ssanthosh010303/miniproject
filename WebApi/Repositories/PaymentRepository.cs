using WebApi.Models;

namespace WebApi.Repositories;

public class PaymentRepository(
    ApplicationDbContext applicationDbContext, ILogger<PaymentRepository> logger)
    : BaseRepository<Payment>(applicationDbContext, logger)
{
}
