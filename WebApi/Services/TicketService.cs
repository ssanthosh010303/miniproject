using Microsoft.EntityFrameworkCore;

using WebApi.Email;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.DataTransferObjects;
using WebApi.Repositories;

namespace WebApi.Services;

public interface ITicketService
{
    Task<Ticket> Add(string jwt);
    Task Delete(int id);
    Task<ICollection<TicketListDto>> GetAll(string username);
    Task<TicketGetDto> GetById(int id);
}

public class TicketService(
        IBaseRepository<Ticket> repository,
        IBaseRepository<UserAccount> userRepository,
        IBaseRepository<Payment> paymentRepository,
        ISeatRepository seatRepository,
        IEmailService emailService,
        IJwtService jwtService,
        ILogger<TicketService> logger)
    : ITicketService
{
    private readonly IBaseRepository<Ticket> _repository = repository;
    private readonly IBaseRepository<Payment> _paymentRepository = paymentRepository;
    private readonly IBaseRepository<UserAccount> _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ISeatRepository _seatRepository = seatRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<TicketService> _logger = logger;

    public async Task<Ticket> Add(string token)
    {
        var claims = _jwtService.ValidateTokenAndGetClaims(token);
        var userAccountId = int.Parse(
            claims.FirstOrDefault(claim => claim.Type == "payer-id")?.Value
            ?? throw new ServiceException("User ID not found in JWT."));
        var paymentId = int.Parse(
            claims.FirstOrDefault(claim => claim.Type == "payment-id")?.Value
            ?? throw new ServiceException("Payment ID not found in JWT."));
        var screenId = int.Parse(
            claims.FirstOrDefault(claim => claim.Type == "payer-id")?.Value
            ?? throw new ServiceException("Screen ID not found in JWT."));
        var movieShowId = int.Parse(
            claims.FirstOrDefault(claim => claim.Type == "movieshow-id")?.Value
            ?? throw new ServiceException("Movie show ID not found in JWT."));
        var seatsBooked = claims.FirstOrDefault(claim => claim.Type == "seats-booked")?.Value
            ?? throw new ServiceException("Seats booked not found in JWT.");

        _logger.LogInformation("Adding a new ticket.");
        try
        {

            var ticket = new Ticket
            {
                UserAccountId = userAccountId,
                PaymentId = paymentId,
                ScreenId = screenId,
                MovieShowId = movieShowId,
                SeatsBooked = seatsBooked
            };

            var temp = await _repository.Add(ticket);
            var addedTicket = await _repository.GetDbSet()
                .Where(entity => entity.Id == temp.Id)
                .Include(entity => entity.UserAccount)
                .Include(entity => entity.MovieShow)
                .ThenInclude(entity => entity.Movie)
                .FirstAsync();

            EmailTemplate ticketBookedEmailTemplate = EmailTemplateFactory.CreateTemplate(
                EmailTemplateType.BookingConfirmation,
                    addedTicket.UserAccount.FullName,
                    addedTicket.MovieShow.Movie.Name,
                    addedTicket.MovieShow.ShowTime.ToString("dd/MM/yyyy HH:mm"),
                    addedTicket.SeatsBooked);

            await _emailService.SendEmail(addedTicket.UserAccount.Email,
                ticketBookedEmailTemplate);

            _logger.LogInformation(
                "Successfully added a new ticket with ID {TicketId}.",
                addedTicket.Id);
            return addedTicket;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while adding a new ticket.");
            throw new ServiceException(
                "An error occurred in the service while adding a new ticket.", ex);
        }
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation(
            "Starting to delete ticket with ID {TicketId}.", id);
        try
        {
            var ticket = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.MovieShow)
                .Include(entity => entity.UserAccount)
                .Include(entity => entity.Payment)
                .FirstAsync();

            if (ticket.MovieShow.ShowTime < DateTime.UtcNow.AddHours(-6))
            {
                throw new ServiceException(
                    "Cannot refund for this ticket. Please read our refund policy.");
            }

            foreach (var seatId in ticket.SeatsBooked.Split(','))
            {
                var seat = await _seatRepository.GetDbSet()
                    .Where(entity => entity.Id == seatId)
                    .FirstAsync();

                seat.UserAccount = null;
                seat.BookedForShow = null;
                await _seatRepository.Update(seat);
            }

            var payment = await _paymentRepository.GetDbSet()
                .Where(entity => entity.Id == ticket.PaymentId)
                .FirstAsync();

            payment.Status = PaymentStatus.Refunded;

            await _paymentRepository.Update(payment);
            await _repository.Delete(id);

            EmailTemplate bookingCancelledTemplate = EmailTemplateFactory.CreateTemplate(
                EmailTemplateType.BookingCancellation,
                ticket.UserAccount.FullName,
                ticket.MovieShow.ShowTime.ToString("dd/MM/yyyy HH:mm"),
                ticket.SeatsBooked);

            await _emailService.SendEmail(ticket.UserAccount.Email, bookingCancelledTemplate);

            _logger.LogInformation(
                "Successfully deleted ticket with ID {TicketId}.", id);
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while deleting the ticket.");
            throw new ServiceException(
                "An error occurred in the service while deleting the ticket.", ex);
        }
    }

    public async Task<ICollection<TicketListDto>> GetAll(string username)
    {
        _logger.LogInformation("Starting to fetch all tickets.");
        try
        {
            var user = await _userRepository.GetDbSet()
                .Where(entity => entity.Username == username)
                .FirstAsync();

            var tickets = await _repository.GetDbSet()
                .Where(entity => entity.UserAccountId == user.Id)
                .Select(entity => new TicketListDto().CopyFrom(entity))
                .ToListAsync();

            _logger.LogInformation(
                "Successfully fetched all tickets. Count: {Count}",
                tickets.Count);
            return tickets;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching all tickets.");
            throw new ServiceException(
                "An error occurred in the service while fetching all tickets.",
                ex);
        }
    }

    public async Task<TicketGetDto> GetById(int id)
    {
        _logger.LogInformation(
            "Starting to fetch ticket with ID {TicketId}.", id);
        try
        {
            var ticket = await _repository.GetDbSet()
                .Where(entity => entity.Id == id)
                .Include(entity => entity.UserAccountId)
                .Include(entity => entity.MovieShow)
                .ThenInclude(show => show.Movie)
                .Select(entity => new TicketGetDto().CopyFrom(entity))
                .FirstAsync();

            _logger.LogInformation(
                "Successfully fetched ticket with ID {TicketId}.", id);
            return ticket;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex,
                "An error occurred in the service while fetching the ticket with ID {TicketId}.",
                id);
            throw new ServiceException(
                $"An error occurred in the service while fetching the ticket with ID {id}.",
                ex);
        }
    }
}
