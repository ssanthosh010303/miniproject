using WebApi.Models;

namespace WebApi.Repositories;

public class MovieRepository(
    ApplicationDbContext applicationDbContext, ILogger<MovieRepository> logger)
    : BaseRepository<Movie>(applicationDbContext, logger)
{
}
