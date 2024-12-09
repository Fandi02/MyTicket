namespace MyTicket.Application.Constant;

public static class EventTypeRabbitMq
{
    public const string CreateUser = "UserCreated";
    public const string UpdateUser = "UserUpdated";
    public const string OrderCreated = "OrderCreated";
    public const string OrderUpdated = "OrderUpdated";
    public const string CancelOrder = "CancelOrder";
}