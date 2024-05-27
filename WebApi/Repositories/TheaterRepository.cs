using WebApi.Models;

namespace WebApi.Repositories;

public class TheaterRepository(
    ApplicationDbContext applicationDbContext, ILogger<TheaterRepository> logger)
    : BaseRepository<Theater>(applicationDbContext, logger)
{
}
