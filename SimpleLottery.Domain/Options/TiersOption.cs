namespace SimpleLottery.Domain.Options;

public class TiersOption
{
    public const string Tiers = "Tiers";  
    public List<TierOption> TiersList { get; set; } = [];
}
