using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;
using MyTicket.Application.Services;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class CancelOrderCommand: IRequest<bool>
    {
        public Guid OrderTicketId { get; set; }
    }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public CancelOrderCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderTicketId == Guid.Empty)
                throw new BadRequestException("Request is null");

            var getOrderticket = await _dbContext.OrderTickets.FirstOrDefaultAsync(x => x.OrderTicketId == request.OrderTicketId && x.IsPaid == false && x.IsDeleted == false);

            if (getOrderticket == null)
                throw new NotFoundException("Order ticket not found");

            var getEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.EventId == getOrderticket.EventId && x.IsDeleted == false);

            if (getEvent == null)
                throw new BadRequestException("Event not found");

            getEvent.AvailableTicket += getOrderticket.Quantity;

            _dbContext.OrderTickets.Remove(getOrderticket);
            _dbContext.Events.Update(getEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var producerUser = new TransactionModel
            {
                EventType = EventTypeRabbitMq.CancelOrder,
                
                EventId = getEvent.EventId,
                Name = getEvent.Name,
                Description = getEvent.Description,
                StartDate = getEvent.StartDate,
                EndDate = getEvent.EndDate,
                TotalTicket = getEvent.TotalTicket,
                AvailableTicket = getEvent.AvailableTicket,
                Price = getEvent.Price,
                Location = getEvent.Location,

                OrderTicketId = getOrderticket.OrderTicketId,
                UserId = getOrderticket.UserId,
                TicketNumber = getOrderticket.TicketNumber,
                Quantity = getOrderticket.Quantity,
                Date = getOrderticket.Date,
                IsPaid = getOrderticket.IsPaid
            };

            var producer = new MessageProducer();
            producer.SendingMessage("cancel_order", producerUser);

            return true;
        }
    }
}