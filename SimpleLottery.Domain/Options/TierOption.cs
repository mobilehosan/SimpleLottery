namespace SimpleLottery.Domain.Options;

public class TierOption
{
    public string Name { get; set; } = string.Empty;
    public bool IsSingleTicketWinner { get; set; }
    public decimal TotalWinningPercent { get; set; }
    public decimal RevenueWinningPercent { get; set; }
}
