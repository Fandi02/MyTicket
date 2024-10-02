namespace MyTicket.Application.Models;
public class ValidationExceptionVm
{
    public object AttemptedValue { get; set; }

    public string[] ErrorMessage { get; set; }
}
