

namespace SimpleLottery.Domain.Entities;

public class Player
{
    public string Name { get; init; }
    public decimal DigitalBalance { get; private set; }
    public int Index { get; init; }
    public bool IsUser { get; init; } = false;

    public int? UserTicketsCount { get; internal set; } = null;

    private Player(string name, int index, decimal digitalBalance, bool isUser = false)
    {
        Name = name;
        Index = index;
        DigitalBalance = digitalBalance;
        IsUser = isUser;
    }

    internal static Player CreatePlayer(string name, int index, decimal balance, bool isUser = false)
    {
        return new Player(name, index, balance, isUser);
    }

    internal bool TryWithdrawTicketCost(decimal cost)
    {
        if (DigitalBalance < cost)
        {
            return false;
        }

        DigitalBalance -= cost;
        return true;
    }

    internal void AddRevenue(decimal revenue)
    {
        DigitalBalance += revenue;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player player &&
               Name == player.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
