using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.Event.Models;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;

namespace MyTicket.Application.Businesses.Event.Queries;
public class GetListEventQuery : BasePagination, IRequest<IEnumerable<GetEventResponse>>
{
    public string? Search { get; set; }
    public int? OrderColumn { get; set; }
    public string? OrderType { get; set; }
}

public class GetListEventQueryHandler : IRequestHandler<GetListEventQuery, IEnumerable<GetEventResponse>>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetListEventQueryHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetEventResponse>> Handle(GetListEventQuery request, CancellationToken cancellationToken)
    {
        var getData = await _dbContext.Events.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            getData = getData.Where(x => x.Name.Contains(request.Search)).ToList();
        }

        switch (request.OrderColumn)
            {
                case 1:

                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Name).ToList();
                    }
                    break;
                case 2:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Description).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Description).ToList();
                    }
                    break;
                case 4:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.StartDate).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.StartDate).ToList();
                    }
                    break;
                case 5:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.EndDate).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.EndDate).ToList();
                    }
                    break;
                case 6:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.AvailableTickets).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.AvailableTickets).ToList();
                    }
                    break;
                case 7:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Price).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Price).ToList();
                    }
                    break;
                case 8:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Location).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Location).ToList();
                    }
                    break;
                default:
                    getData = getData.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
            }

        getData = getData
                    .Skip(request.CalculateOffset())
                    .Take(request.Size).ToList();

        var response = getData.Select(x => new GetEventResponse
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
        .ToList();

        return response;
    }
}