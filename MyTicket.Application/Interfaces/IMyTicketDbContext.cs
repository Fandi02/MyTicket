using Microsoft.EntityFrameworkCore;
using MyTicket.Domain.Entities.Transaction;

namespace MyTicket.Application.Interfaces
{
    public interface IMyTicketDbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<OrderTicket> OrderTickets { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}