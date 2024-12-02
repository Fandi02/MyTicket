using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Extensions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities.Auth;

namespace MyTicket.Application.Businesses.Auth.Commands
{
    public class RegisterCommand: IRequest<string>
    {
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Password { get; set; } = null!;
        public UserRoleEnum Role { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public RegisterCommandHandler(IMyTicketDbContext dbContext, IContext context, IConfiguration configuration, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (
                    string.IsNullOrEmpty(request.Email) || 
                    string.IsNullOrEmpty(request.PhoneNumber) ||
                    string.IsNullOrEmpty(request.UserName) ||
                    string.IsNullOrEmpty(request.FullName) ||
                    string.IsNullOrEmpty(request.BirthDate.ToString()) ||
                    string.IsNullOrEmpty(request.Password)
                )
                throw new BadRequestException("Request is null");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.IsDeleted == false);
            
            if (user != null)
            {
                throw new BadRequestException("Email already exists. Please check your email and try again");
            }

            string stringSalt = SaltGenerator.GetSalt();

            UserPassword saveUserPassword = new UserPassword();

            var saveUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName,
                BirthDate = request.BirthDate,
                Role = request.Role
            };

            saveUserPassword.UserPasswordId = Guid.NewGuid();
            saveUserPassword.UserId = saveUser.UserId;
            saveUserPassword.Salt = Encoding.Default.GetBytes(stringSalt);
            saveUserPassword.Password = (request.Password + saveUserPassword.Salt).ToSHA512();
            saveUserPassword.IsActive = true;
            
            await _dbContext.Users.AddAsync(saveUser);
            await _dbContext.UserPasswords.AddAsync(saveUserPassword);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var subject = "Welcome to MyTicket";
            var link = $"{_configuration["BaseUrl"]}/api/auth/activation?id={saveUser.UserId}";
            var bodyText = $"Hello {request.FullName}, thank you for registering!. Please klik this link for activation -> {link}";

            await _emailSender.SendEmailAsync(request.Email, subject, bodyText);

            return await Task.FromResult(saveUser.Email);
        }
    }
}
