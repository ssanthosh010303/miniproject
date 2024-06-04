/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class MovieRepository(
    ApplicationDbContext applicationDbContext, ILogger<MovieRepository> logger)
    : BaseRepository<Movie>(applicationDbContext, logger)
{
}
