namespace MyTicket.Application.Interfaces;

public interface IMessageProducer
{
    public void SendingMessage(string queueName, object message);
}