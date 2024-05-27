using WebApi.Models;

namespace WebApi.Repositories;

public class UserRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<User>(applicationDbContext)
{
}
