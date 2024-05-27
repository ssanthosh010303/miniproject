using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface IPaymentService
{
    public Task<PaymentAddDto> AddPendingPayment(PaymentAddDto entity, string payerId);
    public Task PaymentSuccessUpdate(PaymentSuccessDto dtoEntity);
    public Task Delete(int id);
    public Task<ICollection<PaymentListDto>> GetAll();
    public Task<PaymentGetDto> GetById(int id);
}

public class PaymentService(
    IBaseRepository<Payment> repository,
    IBaseRepository<Ticket> ticketRepository,
    IBaseRepository<User> userRepository,
    IBaseRepository<Promo> promoRepository,
    ISeatRepository seatRepository,
    IEmailService emailService)
    : IPaymentService
{
    private readonly IBaseRepository<Payment> _repository = repository;
    private readonly IBaseRepository<Ticket> _ticketRepository = ticketRepository;
    private readonly IBaseRepository<Promo> _promoRepository = promoRepository;
    private readonly IBaseRepository<User> _userRepository = userRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;
    private readonly IEmailService _emailService = emailService;

    public async Task<PaymentAddDto> AddPendingPayment(
        PaymentAddDto dtoEntity, string payerUsername)
    {
        try
        {
            var userId = await _userRepository.GetDbSet()
                .Where(entity => entity.Username == payerUsername)
                .Select(entity => entity.Id)
                .FirstAsync();
            var payment = dtoEntity.CopyTo(new Payment());
            var seatIds = dtoEntity.SeatsBooked.Split(',');

            foreach (var seatId in seatIds)
            {
                var seat = await _seatRepository.GetDbSet()
                    .Where(entity =>
                        entity.TheaterId == dtoEntity.TheaterId &&
                        entity.ScreenId == dtoEntity.ScreenId &&
                        entity.Id == seatId)
                    .Include(entity => entity.SeatType)
                    .FirstAsync();

                payment.AmountPaid += seat.SeatType.Price;

                seat.ShowId = dtoEntity.MovieShowId;
                await _seatRepository.Update(seat);
            }

            if (dtoEntity.PromoId != 0)
            {
                var promoEntity = await _promoRepository.GetDbSet()
                    .FindAsync(dtoEntity.PromoId);

                if (promoEntity != null
                    && DateTime.Today <= promoEntity.ValidTo.Date
                    && dtoEntity.Method == promoEntity.AllowedPaymentMethod
                    && payment.AmountPaid >= promoEntity.MinimumPurchase)
                {
                    payment.AmountPaid -= payment.AmountPaid * promoEntity.DiscountPercent / 100;
                    payment.PromoId = promoEntity.Id;
                }
            }

            payment.PayerId = userId;
            await _repository.Add(payment);

            return dtoEntity;
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occured in the service while adding the entity.",
                ex);
        }
        catch (ValidationException ex)
        {
            throw new ServiceValidationException(
                ex.Message,
                ex);
        }
    }

    public async Task PaymentSuccessUpdate(PaymentSuccessDto dtoEntity)
    {
        try
        {
            var payment = await _repository.GetDbSet()
                .Where(entity => entity.Id == dtoEntity.UserId)
                .Include(entity => entity.Promo)
                .FirstAsync();

            // if (payment.Status == PaymentStatus.Success)
            //     throw new ServiceException("Payment already successful.");

            var user = await _userRepository.GetDbSet()
                .FindAsync(dtoEntity.UserId)
                ?? throw new InvalidOperationException("User not found.");
            var ticket = dtoEntity.CopyTo(new Ticket());

            payment.Status = PaymentStatus.Success;
            await _ticketRepository.Add(ticket);

            ticket = await _ticketRepository.GetDbSet()
                .Where(entity => entity.PaymentId == payment.Id)
                .Include(entity => entity.Theater)
                .Include(entity => entity.Screen)
                .Include(entity => entity.MovieShow)
                .ThenInclude(entity => entity.Movie)
                .FirstAsync() ?? throw new ServiceException("Ticket not found.");

            // Send Email
            await _emailService.SendEmailAsync(user.Email, $"Payment Successful, {user.FullName}",
                "Your payment was successful. Here are your ticket details:\n\n"
                + $"Paid Amount: {payment.AmountPaid}\n"
                + $"Screen: {ticket.Screen.Name}\n"
                + $"Theater: {ticket.Theater.Name}\n"
                + $"Movie Name: {ticket.MovieShow.Movie.Name}\n"
                + $"Show Time: {ticket.MovieShow.ShowTime}\n"
                + $"Seats Booked: {ticket.SeatsBooked}\n\n"
                + "With Regards,\nThe Movie Booking Team");

            await _repository.Update(payment);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while updating the entity.",
                ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            await _repository.Delete(id);
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while deleting the entity.",
                ex);
        }
    }

    public async Task<ICollection<PaymentListDto>> GetAll()
    {
        try
        {
            return await _repository.GetDbSet()
                .Select(entity => new PaymentListDto().CopyFrom(entity))
                .ToListAsync();
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching entities.",
                ex);
        }
    }

    public async Task<PaymentGetDto> GetById(int id)
    {
        try
        {
            return new PaymentGetDto().CopyFrom(await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Payer)
                .Include(entity => entity.Promo)
                .FirstAsync());
        }
        catch (RepositoryException ex)
        {
            throw new ServiceException(
                "An error occurred in the service while fetching the entity " +
                $"with ID {id}.", ex);
        }
        catch (InvalidOperationException)
        {
            throw new ServiceException(
                "The entity with the specified ID does not exist.");
        }
    }
}
