namespace SimpleLottery.Domain.Entities;

public class House
{
    public string Name { get; init; }

    public decimal Revenue { get; private set; } = 0;

    private House(string name, decimal revenue)
    {
        Name = name;
        Revenue = revenue;
    }

    internal static House CreateHouse(string name, decimal revenue = 0)
    {
        return new House(name, revenue);
    }

    internal void AddRevenue(decimal revenue)
    {
        Revenue += revenue;
    }
}
