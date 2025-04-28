namespace SimpleLottery.Domain.Options;

public class LotteryOptions
{
    public const string Lottery = "Lottery";
    public int TotalPlayersCountMin { get; set; } = 10;
    public int TotalPlayersCountMax { get; set; } = 15;
    public decimal PlayerStartBalance { get; set; } = 10m;
    public int PlayersTicketsCountMin { get; set; } = 1;
    public int PlayersTicketsCountMax { get; set; } = 10;
    public decimal TicketCost { get; set; } = 1m;
}
