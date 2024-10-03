using MediatR;
using MyTicket.Application.Businesses.UserProfile.Models;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.UserProfile.Queries;
public class GetProfileQuery : IRequest<GetProfileResponse>
{
    public Guid UserId { get; set; }
    public string UserRole { get; set; }
}

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, GetProfileResponse>
{
    private readonly IContext _context;
    private readonly IMyTicketDbContext _dbContext;
    public GetProfileQueryHandler(IContext context, IMyTicketDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<GetProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var response = _dbContext.Users
                        .FirstOrDefault(x => x.UserId == request.UserId && x.IsDeleted == false);
        
        if (response == null)
            throw new BadRequestException("User not found");
        
        return new GetProfileResponse
        {
            UserId = response.UserId,
            Email = response.Email,
            PhoneNumber = response.PhoneNumber,
            FullName = response.FullName,
            UserName = response.UserName,
            Age = response.Age,
            BirthDate = response.BirthDate,
            Role = request.UserRole == "Admin" ? UserRoleEnum.Admin : UserRoleEnum.User
        };
    }
}