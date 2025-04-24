namespace SimpleLottery.Domain.Entities;

public class Ticket
{
    public Guid Id { get; init; }

    public decimal Cost { get; init; }

    private Ticket(Guid id, decimal cost)
    {
        Id = id;
        Cost = cost;
    }

    internal static Ticket CreateTicket(decimal cost)
    {
        return new Ticket(Guid.NewGuid(), cost);
    }
}
