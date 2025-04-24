using SimpleLottery.Domain.Entities;

namespace SimpleLottery.Domain.Aggregates;

public class LotteryResult
{
    public House House { get; init; }

    public decimal HouseRevenue { get; private set; }

    public IReadOnlyDictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> TicketsInfo { get; init; }
    private LotteryResult(House house, decimal houseRevenue, Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> ticketsInfo)
    {
        House = house;
        TicketsInfo = ticketsInfo;
        HouseRevenue = houseRevenue;
    }

    internal static LotteryResult CreateLotteryResult(House house, decimal houseRevenue, Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> ticketsInfo) 
    {
        return new LotteryResult(house, houseRevenue, ticketsInfo);
    }
}
