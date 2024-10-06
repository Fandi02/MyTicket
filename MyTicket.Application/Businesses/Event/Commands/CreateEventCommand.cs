using MediatR;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.Event.Commands
{
    public class CreateEventCommand: IRequest<bool>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AvailableTickets { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public CreateEventCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<bool> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            if (
                    string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.Description) ||
                    request.StartDate != DateTime.MinValue && request.EndDate != DateTime.MinValue && request.EndDate < request.StartDate ||
                    request.AvailableTickets == 0 ||
                    request.Price == 0 ||
                    string.IsNullOrEmpty(request.Location)
                )
                throw new BadRequestException("Request is null");

            var saveEvent = new Domain.Entities.Event
            {
                EventId = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                AvailableTickets = request.AvailableTickets,
                Price = request.Price,
                Location = request.Location,
            };

            await _dbContext.Events.AddAsync(saveEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}