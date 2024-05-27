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
    public Task Delete(int id);
    public Task<ICollection<PaymentListDto>> GetAll();
    public Task<PaymentGetDto> GetById(int id);
}

public class PaymentService(
    IBaseRepository<Payment> repository,
    IBaseRepository<Ticket> ticketRepository,
    IBaseRepository<User> userRepository,
    ISeatRepository seatRepository)
    : IPaymentService
{
    private readonly IBaseRepository<Payment> _repository = repository;
    private readonly IBaseRepository<Ticket> _ticketRepository = ticketRepository;
    private readonly IBaseRepository<User> _userRepository = userRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;

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
                var seat = await _seatRepository.GetById(
                    dtoEntity.TheaterId, dtoEntity.ScreenId, seatId);

                seat.ShowId = dtoEntity.MovieShowId;
                await _seatRepository.Update(seat);
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

    public async Task PaymentSuccessUpdate(int id)
    {
        try
        {
            var payment = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.Promo)
                .FirstAsync();

            payment.Status = PaymentStatus.Success;

            // Create Ticket, Mail User with Ticket, Delete Ticket Flush Job

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
