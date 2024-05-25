using WebApi.Models;

namespace WebApi.Repositories;

public class MovieShowRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<MovieShow>(applicationDbContext)
{
}
