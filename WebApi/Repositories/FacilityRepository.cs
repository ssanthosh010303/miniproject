/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class FacilityRepository(
    ApplicationDbContext applicationDbContext, ILogger<FacilityRepository> logger)
    : BaseRepository<Facility>(applicationDbContext, logger)
{
}
