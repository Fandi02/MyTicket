using Microsoft.EntityFrameworkCore;
using MyTicket.Domain.Entities;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Constant;
using Microsoft.AspNetCore.Http;

namespace MyTicket.Persistence
{
    public class MyTicketDbContext : DbContext, IMyTicketDbContext
    {
        private readonly IContext _context;
        private readonly IClock _clock;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyTicketDbContext(
            DbContextOptions<MyTicketDbContext> options,
            IContext context,
            IHttpContextAccessor httpContextAccessor,
            IClock clock)
            : base(options)
        {
            _context = context;
            _clock = clock;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPassword> UserPasswords { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Event> Events { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var userName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserName)?.Value;
            var fullName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.FullName)?.Value;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedByName = userName;
                        entry.Entity.CreatedByFullName = fullName;
                        entry.Entity.CreatedAt = _clock.CurrentDate();
                        entry.Entity.CreatedAtServer = _clock.CurrentServerDate();
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastUpdatedBy = userId;
                        entry.Entity.LastUpdatedByName = userName;
                        entry.Entity.LastUpdatedByFullName = fullName;
                        entry.Entity.LastUpdatedAt = _clock.CurrentDate();
                        entry.Entity.LastUpdatedAtServer = _clock.CurrentServerDate();
                        break;
                    case EntityState.Deleted:
                        entry.Entity.LastUpdatedBy = userId;
                        entry.Entity.LastUpdatedByName = userName;
                        entry.Entity.LastUpdatedByFullName = fullName;
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
