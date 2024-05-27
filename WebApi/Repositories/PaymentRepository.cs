using WebApi.Models;

namespace WebApi.Repositories;

public class PaymentRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Payment>(applicationDbContext)
{
}
