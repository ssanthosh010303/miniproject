/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class SeatTypeRepository(
    ApplicationDbContext applicationDbContext, ILogger<SeatTypeRepository> logger)
    : BaseRepository<SeatType>(applicationDbContext, logger)
{
}
