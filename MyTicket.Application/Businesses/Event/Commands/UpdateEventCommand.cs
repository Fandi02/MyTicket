using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.Event.Commands
{
    public class UpdateEventCommand: IRequest<bool>
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTicket { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public UpdateEventCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.EventId == Guid.Empty ||
                    string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.Description) ||
                    request.StartDate != DateTime.MinValue && request.EndDate != DateTime.MinValue && request.EndDate < request.StartDate ||
                    request.TotalTicket == 0 ||
                    request.Price == 0 ||
                    string.IsNullOrEmpty(request.Location)
                )
                throw new BadRequestException("Request is null");

            var updateData = await _dbContext.Events.FirstOrDefaultAsync(x => x.EventId == request.EventId && x.IsDeleted == false);

            if (updateData == null)
                throw new NotFoundException("Event not found");

            updateData.Name = request.Name;
            updateData.Description = request.Description;
            updateData.StartDate = request.StartDate;
            updateData.EndDate = request.EndDate;
            updateData.TotalTicket = request.TotalTicket;
            updateData.AvailableTicket = request.TotalTicket - _dbContext.OrderTickets.Where(x => x.EventId == updateData.EventId).Sum(x => x.Quantity);
            updateData.Price = request.Price;
            updateData.Location = request.Location;

            _dbContext.Events.Update(updateData);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}