using WebApi.Models;

namespace WebApi.Repositories;

public class PromoRepository(
    ApplicationDbContext applicationDbContext, ILogger<PromoRepository> logger)
    : BaseRepository<Promo>(applicationDbContext, logger)
{
}
