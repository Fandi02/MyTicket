using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.Payment.Models;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;

namespace MyTicket.Application.Businesses.Payment.Queries;
public class GetListPaymentQuery : BasePagination, IRequest<IEnumerable<GetPaymentQueryResponse>>
{
    public string? Search { get; set; }
    public int OrderColumn { get; set; }
    public string? OrderType { get; set; }
}

public class GetListPaymentQueryHandler : IRequestHandler<GetListPaymentQuery, IEnumerable<GetPaymentQueryResponse>>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetListPaymentQueryHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetPaymentQueryResponse>> Handle(GetListPaymentQuery request, CancellationToken cancellationToken)
    {  
        var getData = await _dbContext.Payments.Include(x => x.User).Include(x => x.OrderTicket.Event).Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            getData = getData.Where(x => x.OrderTicket.Event.Name.Contains(request.Search) || x.User.Email.Contains(request.Search)).ToList();
        }

        switch (request.OrderColumn)
            {
                case 1:

                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.OrderTicket.Event.Name).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.OrderTicket.Event.Name).ToList();
                    }
                    break;
                case 2:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.User.Email).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.User.Email).ToList();
                    }
                    break;
                case 4:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.User.FullName).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.User.FullName).ToList();
                    }
                    break;
                case 5:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Description).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Description).ToList();
                    }
                    break;
                case 6:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.TotalPayment).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.TotalPayment).ToList();
                    }
                    break;
                case 7:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Date).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Date).ToList();
                    }
                    break;
                case 8:
                    if (request.OrderType.ToLower() == "asc")
                    {
                        getData = getData.OrderBy(x => x.Status).ToList();
                    }
                    else
                    {
                        getData = getData.OrderByDescending(x => x.Status).ToList();
                    }
                    break;
                default:
                    getData = getData.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
            }

        getData = getData
                    .Skip(request.CalculateOffset())
                    .Take(request.Size).ToList();

        var response = getData.Select(x => new GetPaymentQueryResponse
        {
            PaymentId = x.PaymentId,
            EventId = x.OrderTicket.EventId,
            EventName = x.OrderTicket.Event.Name,
            UserId = x.UserId,
            Email = x.User.Email,
            FullName = x.User.FullName,
            ImagePayment = x.ImagePayment,
            Description = x.Description,
            TotalPayment = x.TotalPayment,
            Date = x.Date,
            Status = x.Status,

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