using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.OrderTicket.Models;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;

namespace MyTicket.Application.Businesses.OrderTicket.Queries;
public class GetListOrderQuery : BasePagination, IRequest<IEnumerable<GetOrderQueryResponse>>
{
    public string? Search { get; set; }
    public int OrderColumn { get; set; }
    public string? OrderType { get; set; }
}

public class GetListOrderQueryHandler : IRequestHandler<GetListOrderQuery, IEnumerable<GetOrderQueryResponse>>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetListOrderQueryHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetOrderQueryResponse>> Handle(GetListOrderQuery request, CancellationToken cancellationToken)
    {  
        var getData = await _dbContext.OrderTickets.Include(x => x.Event).Where(x => x.IsPaid == false && x.IsDeleted == false).ToListAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            getData = getData.Where(x => x.Event.Name.Contains(request.Search)).ToList();
        }

        switch (request.OrderColumn)
            {
                case 1:

                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Event.Name).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Event.Name).ToList();
                    }
                    break;
                case 2:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.TicketNumber).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.TicketNumber).ToList();
                    }
                    break;
                case 4:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Quantity).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Quantity).ToList();
                    }
                    break;
                case 5:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Price).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Price).ToList();
                    }
                    break;
                case 6:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Date).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Date).ToList();
                    }
                    break;
                default:
                    getData = getData.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
            }

        getData = getData
                    .Skip(request.CalculateOffset())
                    .Take(request.Size).ToList();

        var response = getData.Select(x => new GetOrderQueryResponse
        {
            OrderTicketId = x.OrderTicketId,
            UserId = x.UserId,
            EventId = x.EventId,
            EventName = x.Event.Name,
            TicketNumber = x.TicketNumber,
            Quantity = x.Quantity,
            Price = x.Price,
            Date = x.Date,

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