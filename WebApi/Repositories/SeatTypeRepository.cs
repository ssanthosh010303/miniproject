using WebApi.Models;

namespace WebApi.Repositories;

public class SeatTypeRepository(
    ApplicationDbContext applicationDbContext, ILogger<SeatTypeRepository> logger)
    : BaseRepository<SeatType>(applicationDbContext, logger)
{
}
