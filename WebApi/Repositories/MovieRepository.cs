using WebApi.Models;

namespace WebApi.Repositories;

public class MovieRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Movie>(applicationDbContext)
{
}
