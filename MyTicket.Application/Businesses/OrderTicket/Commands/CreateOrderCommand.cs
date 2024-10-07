using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class CreateOrderCommand: IRequest<bool>
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public CreateOrderCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.EventId == Guid.Empty ||
                    request.UserId == Guid.Empty ||
                    request.Quantity == 0
                )
                throw new BadRequestException("Request is null");

            var getEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.EventId == request.EventId && x.IsDeleted == false);

            if (getEvent == null)
                throw new BadRequestException("Event not found");

            if (getEvent.AvailableTickets <= 0 || getEvent.AvailableTickets < request.Quantity)
                throw new BadRequestException("Not enough tickets available");

            var saveOrder = new Domain.Entities.OrderTicket
            {
                OrderTicketId = Guid.NewGuid(),
                EventId = request.EventId,
                UserId = request.UserId,
                TicketNumber = GenerateRandomString(12),
                Quantity = request.Quantity,
                Price = getEvent.Price * request.Quantity,
                Date = _clock.CurrentDate(),
                IsPaid = false
            };

            getEvent.AvailableTickets -= request.Quantity;

            await _dbContext.OrderTickets.AddAsync(saveOrder);
            _dbContext.Events.Update(getEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }
    }
}