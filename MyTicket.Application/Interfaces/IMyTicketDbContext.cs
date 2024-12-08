using Microsoft.EntityFrameworkCore;
using MyTicket.Domain.Entities.Auth;
using MyTicket.Domain.Entities.Payment;
using MyTicket.Domain.Entities.Transaction;

namespace MyTicket.Application.Interfaces
{
    public interface IMyTicketDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<OrderTicket> OrderTickets { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}