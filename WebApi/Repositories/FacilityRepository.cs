using WebApi.Models;

namespace WebApi.Repositories;

public class FacilityRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Facility>(applicationDbContext)
{
}
