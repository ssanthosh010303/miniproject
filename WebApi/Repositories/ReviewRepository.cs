using WebApi.Models;

namespace WebApi.Repositories;

public class ReviewRepository(
    ApplicationDbContext applicationDbContext, ILogger<ReviewRepository> logger)
    : BaseRepository<Review>(applicationDbContext, logger)
{
}
