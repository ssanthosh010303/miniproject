using WebApi.Models;

namespace WebApi.Repositories;

public class ReviewRepository(
    ApplicationDbContext applicationDbContext) : BaseRepository<Review>(applicationDbContext)
{
}
