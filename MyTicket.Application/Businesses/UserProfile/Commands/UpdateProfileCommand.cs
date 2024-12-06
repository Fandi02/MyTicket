using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Services;

namespace MyTicket.Application.Businesses.Auth.Commands
{
    public class UpdateProfileCommand: IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public UpdateProfileCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.UserId == Guid.Empty ||
                    string.IsNullOrEmpty(request.PhoneNumber) ||
                    string.IsNullOrEmpty(request.UserName) ||
                    string.IsNullOrEmpty(request.FullName) ||
                    string.IsNullOrEmpty(request.BirthDate.ToString())
                )
                throw new BadRequestException("Request is null");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.IsDeleted == false);
            
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }

            user.PhoneNumber = request.PhoneNumber;
            user.FullName = request.FullName;
            user.UserName = request.UserName;
            user.BirthDate = request.BirthDate;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (user.IsActivate)
            {
                var producerUser = new 
                {
                    EventType = EventTypeRabbitMq.UpdateUser,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    BirthDate = user.BirthDate,
                    Role = user.Role
                };

                var producer = new MessageProducer();
                producer.SendingMessage("user_updated", producerUser);
            }

            return true;
        }
    }
}
