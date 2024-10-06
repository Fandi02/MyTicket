using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.Event.Commands
{
    public class DeleteEventCommand: IRequest<bool>
    {
        public Guid EventId { get; set; }
    }

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public DeleteEventCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.EventId == Guid.Empty
                )
                throw new BadRequestException("Request is null");

            var deleteData = await _dbContext.Events.FirstOrDefaultAsync(x => x.EventId == request.EventId && x.IsDeleted == false);

            if (deleteData == null)
                throw new NotFoundException("Event not found");

            _dbContext.Events.Remove(deleteData);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}