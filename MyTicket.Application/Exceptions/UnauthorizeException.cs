namespace MyTicket.Application.Exceptions;
public class UnauthorizeException : Exception
{
    public UnauthorizeException() : base("Unauthorized")
    {
    }

    public UnauthorizeException(string message) : base(message)
    {
    }

    public UnauthorizeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
