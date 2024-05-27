using WebApi.Models;

namespace WebApi.Repositories;

public class ScreenRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Screen>(applicationDbContext)
{
}
