using WebApi.Models;

namespace WebApi.Repositories;

public class MovieShowRepository(
    ApplicationDbContext applicationDbContext, ILogger<MovieShowRepository> logger)
    : BaseRepository<MovieShow>(applicationDbContext, logger)
{
}
