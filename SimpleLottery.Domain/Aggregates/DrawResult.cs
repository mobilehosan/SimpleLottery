namespace SimpleLottery.Domain.Aggregates;

public class DrawResult
{
    public decimal HouseRevenue { get; init; }
    public IReadOnlyList<WinningTicket> WinningTickets { get; init; }
    internal DrawResult(decimal houseRevenue, List<WinningTicket> winningTickets)
    {
        HouseRevenue = houseRevenue;
        WinningTickets = winningTickets;
    }
}
