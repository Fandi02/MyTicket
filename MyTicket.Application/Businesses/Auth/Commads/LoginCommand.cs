using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Extensions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;
using MyTicket.Application.Businesses.Auth.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.Application.Businesses.Auth.Commands
{
    public class LoginCommand: IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ApplicationCode { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public LoginCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "MyUser",
                Password = "MyPassword",
                VirtualHost = "/"
            };

            var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            channel.QueueDeclare("AuthQueue", durable: false, exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"A message has been received - {message}");
            };

            if (string.IsNullOrEmpty(request.Email))
                throw new BadRequestException("Email not empty");

            if (string.IsNullOrEmpty(request.Password))
                throw new BadRequestException("Password not empty");

            if (string.IsNullOrEmpty(request.ApplicationCode))
                throw new BadRequestException("Application code not empty");
                
            var responseUser = await _dbContext.Users.Include(x => x.UserRoles).Include(x => x.UserPasswords).FirstOrDefaultAsync(x => x.Email == request.Email && x.IsDeleted == false);
            
            if (responseUser == null)
            {
                throw new BadRequestException("Email not found. Please check your email and try again");
            }

            var role = "";

            if (request.ApplicationCode == "Login.Admin")
            {
                if (responseUser.UserRoles != null && !responseUser.UserRoles.Any(x => x.Role == UserRoleEnum.Admin))
                {
                    throw new BadRequestException("Please check your account role and try again");
                } 
                
                role = UserRoleEnum.Admin.ToString();
            } 
            else if (request.ApplicationCode == "Login.Client")
            {
                if (responseUser.UserRoles != null && !responseUser.UserRoles.Any(x => x.Role == UserRoleEnum.User))
                {
                    throw new BadRequestException("Please check your account role and try again");
                } 
                
                role = UserRoleEnum.Admin.ToString();
            }
            else 
            {
                throw new BadRequestException("Please check your account role and try again");
            }

            var userPassword = await _dbContext.UserPasswords.FirstOrDefaultAsync(x => x.UserId == responseUser.UserId && x.IsActive == true && x.IsDeleted == false);

            if (userPassword == null)
            {
                throw new BadRequestException("Password not found. Please check your password and try again");
            }

            var requestPasswordSHA = (request.Password + userPassword.Salt).ToSHA512();

            if (requestPasswordSHA != userPassword.Password)
            {
                throw new BadRequestException("Password not found. Please check your password and try again");
            }

            LoginResponse response = new LoginResponse
            {
                UserId = responseUser.UserId,
                FullName = responseUser.FullName,
                UserName = responseUser.UserName,
                Role = role
            };

            channel.BasicConsume("AuthQueue", true, consumer);

            Console.ReadKey();

            return response;
        }
    }
}
