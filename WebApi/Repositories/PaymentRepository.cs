/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using WebApi.Models;

namespace WebApi.Repositories;

public class PaymentRepository(
    ApplicationDbContext applicationDbContext, ILogger<PaymentRepository> logger)
    : BaseRepository<Payment>(applicationDbContext, logger)
{
}
