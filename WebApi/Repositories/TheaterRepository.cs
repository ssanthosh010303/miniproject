using WebApi.Models;

namespace WebApi.Repositories;

public class TheaterRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Theater>(applicationDbContext)
{
}
