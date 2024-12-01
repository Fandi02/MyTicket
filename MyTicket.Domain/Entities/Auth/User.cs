using MyTicket.Domain.Entities.Transaction;

namespace MyTicket.Domain.Entities.Auth;

public enum UserRoleEnum
{
    Admin,
    User
}

public class User : BaseEntity
{
    public Guid UserId { get; set; } = new Guid();
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int Age { get; set; } = 0;
    public DateTime BirthDate { get; set; }
    public bool IsActivate { get; set; }
    public UserRoleEnum Role { get; set; }

    public ICollection<UserPassword>? UserPasswords { get; set; }
    public ICollection<OrderTicket>? OrderTickets { get; set; }
}