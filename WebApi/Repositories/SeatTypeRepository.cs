using WebApi.Models;

namespace WebApi.Repositories;

public class SeatTypeRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<SeatType>(applicationDbContext)
{
}
