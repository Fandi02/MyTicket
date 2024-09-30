namespace MyTicket.Domain.Entities;

public class UserPassword : BaseEntity
{
    public Guid UserPasswordId { get; set; } = new Guid();
    public Guid UserId { get; set; }
    public string Password { get; set; } = null!;
    public byte[] Salt { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime NotActiveUntil { get; set; }
    public User User { get; set; } = null!;
}