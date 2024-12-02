using Microsoft.EntityFrameworkCore;
using MyTicket.Domain.Entities.Auth;

namespace MyTicket.Application.Interfaces
{
    public interface IMyTicketDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPassword> UserPasswords { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}