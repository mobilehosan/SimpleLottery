using SimpleLottery.Domain.Entities;

namespace SimpleLottery.Domain.Aggregates;

public class LotteryResult
{
    public House House { get; init; }

    public bool LastDraw { get; private set; } = false;

    public decimal HouseRevenue { get; private set; }

    public IReadOnlyDictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> TicketsInfo { get; init; }
    private LotteryResult(
        House house, 
        decimal houseRevenue, 
        Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> ticketsInfo,
        bool lastDraw = false)
    {
        House = house;
        TicketsInfo = ticketsInfo;
        HouseRevenue = houseRevenue;
        LastDraw = lastDraw;
    }

    internal static LotteryResult CreateLotteryResult(House house, decimal houseRevenue, Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> ticketsInfo) 
    {
        return new LotteryResult(house, houseRevenue, ticketsInfo);
    }

    internal static LotteryResult CreateEmptyLotteryResult(House house, IReadOnlyList<Player> players)
    {
        return new LotteryResult(
            house,
            0m,
            players.ToDictionary(player => player, list => (new List<Ticket>(), new List<WinningTicket>())), 
            true);
    }
}
