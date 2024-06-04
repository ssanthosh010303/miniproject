/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class ReviewRepository(
    ApplicationDbContext applicationDbContext, ILogger<ReviewRepository> logger)
    : BaseRepository<Review>(applicationDbContext, logger)
{
}
