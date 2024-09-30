namespace MyTicket.Application.Interfaces;

public interface IClock
{
    DateTime CurrentDate();

    DateTime CurrentServerDate();
}