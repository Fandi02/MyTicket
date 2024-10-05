namespace MyTicket.WebApi.ServiceMessageBroker;

public interface IMessageProducer
{
    public void SendingMessage(string queueName, object message);
}