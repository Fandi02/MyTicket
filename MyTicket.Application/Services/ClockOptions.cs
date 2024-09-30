namespace MyTicket.Application.Services;

public sealed class ClockOptions
{
    public ClockOptions()
    {
        Hours = 7;
    }

    public int Hours { get; set; }
}