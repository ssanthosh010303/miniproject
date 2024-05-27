using WebApi.Models;

namespace WebApi.Repositories;

public class UserRepository(
    ApplicationDbContext applicationDbContext, ILogger<UserRepository> logger)
    : BaseRepository<User>(applicationDbContext, logger)
{
}
