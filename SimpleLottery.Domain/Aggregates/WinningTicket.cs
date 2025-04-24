using SimpleLottery.Domain.Entities;
using SimpleLottery.Domain.Options;

namespace SimpleLottery.Domain.Aggregates;

public class WinningTicket
{
    public Ticket Ticket { get; init; }
    public Player Player { get; init; }
    public TierOption Tier { get; init; }
    public decimal Ammount { get; init; }

    internal WinningTicket(Ticket ticket, Player player, TierOption tier, decimal ammount)
    {
        Ticket = ticket;
        Player = player;
        Tier = tier;
        Ammount = ammount;
    }
}
