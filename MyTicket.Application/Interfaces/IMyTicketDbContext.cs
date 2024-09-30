using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Interfaces
{
    public interface IMyTicketDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPassword> UserPasswords { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}