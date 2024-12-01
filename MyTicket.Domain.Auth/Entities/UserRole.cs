namespace MyTicket.Domain.Entities;

public enum UserRoleEnum
{
    Admin,
    User
}

public class UserRole : BaseEntity
{
    public Guid UserRoleId { get; set; } = new Guid();
    public Guid UserId { get; set; }
    public UserRoleEnum Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime NotActiveUntil { get; set; }
    public User User { get; set; } = null!;
}