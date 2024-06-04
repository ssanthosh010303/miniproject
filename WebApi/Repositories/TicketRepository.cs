/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class TicketRepository(
    ApplicationDbContext applicationDbContext, ILogger<TicketRepository> logger)
    : BaseRepository<Ticket>(applicationDbContext, logger)
{
}
