/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class TheaterRepository(
    ApplicationDbContext applicationDbContext, ILogger<TheaterRepository> logger)
    : BaseRepository<Theater>(applicationDbContext, logger)
{
}
