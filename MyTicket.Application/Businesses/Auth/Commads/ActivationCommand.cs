using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Services;

namespace MyTicket.Application.Businesses.Auth.Commands
{
    public class ActivationCommand: IRequest<string>
    {
        public Guid UserId { get; set; }
    }

    public class ActivationCommandHandler : IRequestHandler<ActivationCommand, string>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public ActivationCommandHandler(IMyTicketDbContext dbContext, IContext context, IConfiguration configuration, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<string> Handle(ActivationCommand request, CancellationToken cancellationToken)
        {
            var getUser = _dbContext.Users.Where(x => x.UserId == request.UserId && x.IsDeleted == false).FirstOrDefault();

            if (getUser == null)
                throw new BadRequestException("User not found");

            if (!getUser.IsActivate)
            {
                var producerUser = new 
                {
                    EventType = EventTypeRabbitMq.CreateUser,
                    UserId = getUser.UserId,
                    Email = getUser.Email,
                    PhoneNumber = getUser.PhoneNumber,
                    FullName = getUser.FullName,
                    UserName = getUser.UserName,
                    BirthDate = getUser.BirthDate,
                    Role = getUser.Role
                };

                var producer = new MessageProducer();
                producer.SendingMessage("user_created", producerUser);
            }

            getUser.IsActivate = true;

            _dbContext.Users.Update(getUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return "OK";
        }
    }
}
