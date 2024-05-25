using WebApi.Models;

namespace WebApi.Repositories;

public class TicketRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<Ticket>(applicationDbContext)
{
}
