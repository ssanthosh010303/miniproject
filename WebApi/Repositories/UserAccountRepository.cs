/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class UserAccountRepository(
    ApplicationDbContext applicationDbContext, ILogger<UserAccountRepository> logger)
    : BaseRepository<UserAccount>(applicationDbContext, logger)
{
}
