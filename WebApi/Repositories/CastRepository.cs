using WebApi.Models;

namespace WebApi.Repositories;

public class CastRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Cast>(applicationDbContext)
{
}
