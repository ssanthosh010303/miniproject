/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

using WebApi.Email;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IPaymentService
{
    public Task<PaymentAddSuccessDto> RegisterPayment(
        string username, PaymentAddDto entity);
}

public class PaymentService(
        IBaseRepository<Payment> repository,
        IBaseRepository<Promo> promoRepository,
        IBaseRepository<UserAccount> userAccountRepository,
        ISeatRepository seatRepository,
        IJwtService jwtService,
        ILogger<PaymentService> logger)
    : IPaymentService
{
    private readonly IBaseRepository<Payment> _repository = repository;
    private readonly IBaseRepository<Promo> _promoRepository = promoRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;
    private readonly IBaseRepository<UserAccount> _userAccountRepository = userAccountRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<PaymentAddSuccessDto> RegisterPayment(
        string username, PaymentAddDto dtoEntity)
    {
        _logger.LogInformation(
            "Registering payment for user with ID {Username}.", username);

        UserAccount userAccount = await _userAccountRepository.GetDbSet()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync()
            ?? throw new ServiceException("User not found.");

        double payableAmount = await CalculatePriceAndTemporarilyBlockSeats(
            dtoEntity, userAccount);

        Payment payment = dtoEntity.CopyTo(new Payment());

        payment.PayerId = userAccount.Id;
        payment.AmountPaid = payableAmount;
        payment = await _repository.Add(payment);

        Claim[] additionalClaims =
        [
            new Claim("payer-id", userAccount.Id.ToString()),
            new Claim("payment-id", payment.Id.ToString()),
            new Claim("seats-booked", dtoEntity.SeatsBooked),
            new Claim("screen-id", dtoEntity.ScreenId.ToString()),
            new Claim("movieshow-id", dtoEntity.MovieShowId.ToString())
        ];

        string token = _jwtService.GenerateToken(
            username, "ClientChallenge", additionalClaims: additionalClaims)[0];

        return new PaymentAddSuccessDto(token, payableAmount);
    }

    private async Task<double> CalculatePriceAndTemporarilyBlockSeats(
        PaymentAddDto entity, UserAccount userAccount)
    {
        double payableAmount = 0.0;
        List<string> seatIdsList = [.. entity.SeatsBooked.Split(',')];

        Promo promo = await _promoRepository.GetDbSet()
            .Where(p => p.Id == entity.PromoId)
            .FirstOrDefaultAsync();

        try
        {
            foreach (var seatId in seatIdsList)
            {
                Seat seat = await _seatRepository.GetDbSet()
                    .Include(s => s.SeatType)
                    .Where(s => s.ScreenId == entity.ScreenId && s.Id == seatId)
                    .FirstOrDefaultAsync()
                    ?? throw new RepositoryException("Seat not found.");

                if (seat.TemporaryLockUntil != null
                    && seat.TemporaryLockUntil > DateTime.UtcNow)
                {
                    throw new RepositoryException(
                        "Seat under booking process. Please try again later.");
                }

                payableAmount += seat.SeatType.Price;
                seat.TemporaryLockUntil = DateTime.UtcNow.AddMinutes(5);
                seat.BookedForShowId = entity.MovieShowId;
                seat.UserAccount = userAccount;

                await _seatRepository.Update(seat);
            }

        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex.Message);
            throw new ServiceException(ex.Message, ex);
        }

        if (promo != null
            && promo.ValidFrom <= DateTime.UtcNow
            && promo.ValidTo >= DateTime.UtcNow
            && promo.MinimumPurchase <= payableAmount
            && promo.AllowedPaymentMethod == entity.Method)
        {
            payableAmount *= 1 - promo.DiscountPercent / 100;
        }

        return payableAmount;
    }
}
