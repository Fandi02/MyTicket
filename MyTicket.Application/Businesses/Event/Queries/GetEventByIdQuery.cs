using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.Event.Models;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Businesses.Event.Queries;
public class GetEventByIdQuery : IRequest<GetEventResponse>
{
    public Guid EventId { get; set; }
}

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventResponse>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetEventByIdQueryHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<GetEventResponse> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _dbContext.Events
                            .Where(x => x.EventId == request.EventId && x.IsDeleted == false)
                            .Select(x => new GetEventResponse
                            {
                                EventId = x.EventId,
                                Name = x.Name,
                                Description = x.Description,
                                StartDate = x.StartDate,
                                EndDate = x.EndDate,
                                AvailableTickets = x.AvailableTickets,
                                Price = x.Price,
                                Location = x.Location,

                                CreatedBy = x.CreatedBy,
                                CreatedByName = x.CreatedByName,
                                CreatedByFullName = x.CreatedByFullName,
                                CreatedAt = x.CreatedAt,
                                CreatedAtServer = x.CreatedAtServer,
                                LastUpdatedBy = x.LastUpdatedBy,
                                LastUpdatedByName = x.LastUpdatedByName,
                                LastUpdatedByFullName = x.LastUpdatedByFullName,
                                LastUpdatedAt = x.LastUpdatedAt,
                                LastUpdatedAtServer = x.LastUpdatedAtServer
                            })
                            .FirstOrDefaultAsync(cancellationToken);

        if (response == null)
            throw new NotFoundException("Event not found");

        return response;
    }
}