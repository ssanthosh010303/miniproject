/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class ScreenRepository(
    ApplicationDbContext applicationDbContext, ILogger<ScreenRepository> logger)
    : BaseRepository<Screen>(applicationDbContext, logger)
{
}
