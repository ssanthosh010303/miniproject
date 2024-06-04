/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class MovieShowRepository(
    ApplicationDbContext applicationDbContext, ILogger<MovieShowRepository> logger)
    : BaseRepository<MovieShow>(applicationDbContext, logger)
{
}
