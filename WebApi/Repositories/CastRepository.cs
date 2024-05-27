using WebApi.Models;

namespace WebApi.Repositories;

public class CastRepository(
    ApplicationDbContext applicationDbContext, ILogger<CastRepository> logger)
    : BaseRepository<Cast>(applicationDbContext, logger)
{
}
