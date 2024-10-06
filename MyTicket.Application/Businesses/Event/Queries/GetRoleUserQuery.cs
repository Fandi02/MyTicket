using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.Event.Models;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.Event.Queries;
public class GetRoleUser : IRequest<IEnumerable<GetRoleUserResponse>>
{
}

public class GetRoleUserHandler : IRequestHandler<GetRoleUser, IEnumerable<GetRoleUserResponse>>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetRoleUserHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetRoleUserResponse>> Handle(GetRoleUser request, CancellationToken cancellationToken)
    {
        var response = await _dbContext.Users
                            .Include(x => x.UserRoles)
                            .Where(x => x.UserRoles.Role == UserRoleEnum.User && x.IsDeleted == false)
                            .Select(x => new GetRoleUserResponse
                            {
                                Email = x.Email,
                                FullName = x.FullName
                            })
                            .ToListAsync(cancellationToken);

        if (response.Count == 0)
            throw new NotFoundException("User not found");

        return response;
    }
}