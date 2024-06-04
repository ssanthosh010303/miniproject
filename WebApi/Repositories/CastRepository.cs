/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class CastRepository(
    ApplicationDbContext applicationDbContext, ILogger<CastRepository> logger)
    : BaseRepository<Cast>(applicationDbContext, logger)
{
}
