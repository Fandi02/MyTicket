using Microsoft.EntityFrameworkCore;
using MyTicket.Domain.Entities;
using MyTicket.Application.Interfaces;

namespace MyTicket.Persistence
{
    public class MyTicketDbContext : DbContext, IMyTicketDbContext
    {
        private readonly IContext _context;
        private readonly IClock _clock;

        public MyTicketDbContext(
            DbContextOptions<MyTicketDbContext> options,
            IContext context,
            IClock clock)
            : base(options)
        {
            _context = context;
            _clock = clock;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPassword> UserPasswords { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Event> Events { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _context.UserId;
                        entry.Entity.CreatedByName = _context.UserName;
                        entry.Entity.CreatedByFullName = _context.FullName;
                        entry.Entity.CreatedAt = _clock.CurrentDate();
                        entry.Entity.CreatedAtServer = _clock.CurrentServerDate();
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastUpdatedBy = _context.UserId;
                        entry.Entity.LastUpdatedByName = _context.UserName;
                        entry.Entity.LastUpdatedByFullName = _context.FullName;
                        entry.Entity.LastUpdatedAt = _clock.CurrentDate();
                        entry.Entity.LastUpdatedAtServer = _clock.CurrentServerDate();
                        break;
                    case EntityState.Deleted:
                        entry.Entity.LastUpdatedBy = _context.UserId;
                        entry.Entity.LastUpdatedByName = _context.UserName;
                        entry.Entity.LastUpdatedByFullName = _context.FullName;
                        entry.Entity.LastUpdatedAt = _clock.CurrentDate();
                        entry.Entity.LastUpdatedAtServer = _clock.CurrentServerDate();
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
