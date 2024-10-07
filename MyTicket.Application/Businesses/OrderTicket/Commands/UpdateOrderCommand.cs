using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class UpdateOrderCommand: IRequest<bool>
    {
        public Guid OrderTicketId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public UpdateOrderCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.OrderTicketId == Guid.Empty ||
                    request.Quantity == 0
                )
                throw new BadRequestException("Request is null");

            var getOrderticket = await _dbContext.OrderTickets.FirstOrDefaultAsync(x => x.OrderTicketId == request.OrderTicketId && x.IsPaid == false && x.IsDeleted == false);

            if (getOrderticket == null)
                throw new NotFoundException("Order ticket not found");

            var getEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.EventId == getOrderticket.EventId && x.IsDeleted == false);

            if (getEvent == null)
                throw new BadRequestException("Event not found");

            if (getOrderticket.Quantity == request.Quantity)
            {
                throw new BadRequestException("Quantity not change");
            }
            else if (getOrderticket.Quantity > request.Quantity)
            {
                getEvent.AvailableTickets += getOrderticket.Quantity - request.Quantity;

                getOrderticket.Quantity = request.Quantity;
                getOrderticket.Price = getEvent.Price * request.Quantity;
            } 
            else if (getOrderticket.Quantity < request.Quantity)
            {
                if (getEvent.AvailableTickets == 0 || getEvent.AvailableTickets < request.Quantity - getOrderticket.Quantity)
                    throw new BadRequestException("Not enough tickets available");

                getEvent.AvailableTickets -= request.Quantity - getOrderticket.Quantity;

                getOrderticket.Quantity = request.Quantity;
                getOrderticket.Price = getEvent.Price * request.Quantity;
            }
            else 
            {
                throw new BadRequestException("Quantity not change");
            }

            getOrderticket.Date = _clock.CurrentDate();

            _dbContext.Events.Update(getEvent);
            _dbContext.OrderTickets.Update(getOrderticket);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}