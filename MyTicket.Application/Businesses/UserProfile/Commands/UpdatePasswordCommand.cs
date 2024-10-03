using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Extensions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.Auth.Commands
{
    public class UpdatePasswordCommand: IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public UpdatePasswordCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.UserId == Guid.Empty ||
                    string.IsNullOrEmpty(request.PasswordOld) ||
                    string.IsNullOrEmpty(request.PasswordNew)
                )
                throw new BadRequestException("Request is null");

            var userPassword = await _dbContext.UserPasswords.Where(x => x.UserId == request.UserId && x.IsDeleted == false).ToListAsync();
            
            if (userPassword == null || userPassword.Count == 0)
                throw new BadRequestException("User password not found");

            var passwordOldActive = userPassword.Where(x => x.IsActive == true && x.Password == (request.PasswordOld + x.Salt).ToSHA512() && x.IsDeleted == false).FirstOrDefault();

            if (passwordOldActive == null)
                throw new BadRequestException("Password Old not match");

            if (userPassword.Any(x => x.Password == (request.PasswordNew + x.Salt).ToSHA512()))
                throw new BadRequestException("Password has been used before, change password");

            passwordOldActive.IsActive = false;

            UserPassword saveUserPassword = new UserPassword();
            string stringSalt = SaltGenerator.GetSalt();

            saveUserPassword.UserPasswordId = Guid.NewGuid();
            saveUserPassword.UserId = request.UserId;
            saveUserPassword.Salt = Encoding.Default.GetBytes(stringSalt);
            saveUserPassword.Password = (request.PasswordNew + saveUserPassword.Salt).ToSHA512();
            saveUserPassword.IsActive = true;


            _dbContext.UserPasswords.Update(passwordOldActive);
            await _dbContext.UserPasswords.AddAsync(saveUserPassword);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}