using WebApi.Models;

namespace WebApi.Repositories;

public class PromoRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Promo>(applicationDbContext)
{

}
