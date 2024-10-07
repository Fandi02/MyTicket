namespace MyTicket.Domain.Entities;

public class User : BaseEntity
{
    public Guid UserId { get; set; } = new Guid();
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int Age { get; set; } = 0;
    public DateTime BirthDate { get; set; }

    public ICollection<UserPassword>? UserPasswords { get; set; }
    public UserRole? UserRoles { get; set; }
    public ICollection<OrderTicket>? orderTicket { get; set; }
}