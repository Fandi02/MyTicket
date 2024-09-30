using MyTicket.Application.Interfaces;

namespace MyTicket.Application.Services;

public sealed class ClockSevice : IClock
{
    private readonly ClockOptions _options;

    public ClockSevice(ClockOptions options)
    {
        _options = options;
    }

    public DateTime CurrentDate() => DateTime.UtcNow;

    public DateTime CurrentServerDate() => CurrentDate().AddHours(_options.Hours);
}