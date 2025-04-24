using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Entities;
using SimpleLottery.Domain.Options;

namespace SimpleLottery.Domain.Services.DrawStrategies;

class SimpleDrawStrategy(IRandomizer randomizer) : IDrawStrategy
{
    public DrawResult Draw(List<(Player Player, Ticket Ticket)> tickets, TiersOption tiers)
    {
        var totalTicketsCost = tickets.Sum(x => x.Ticket.Cost);
        var totalTicketsCount = tickets.Count;
        var currentTicketsCost = totalTicketsCost;

        List<WinningTicket> winningTickets = DetermineWinningTickets(tickets, tiers, totalTicketsCount, totalTicketsCost, ref currentTicketsCost);

        return new DrawResult(currentTicketsCost, winningTickets);
    }

    private List<WinningTicket> DetermineWinningTickets(
        List<(Player Player, Ticket Ticket)> tickets,
        TiersOption tiers,
        int totalTicketsCount,
        decimal totalTicketsCost,
        ref decimal currentTicketsCost)
    {
        List<WinningTicket> winners = [];
        foreach (var tier in tiers.TiersList.OrderByDescending(x => x.IsSingleTicketWinner)
                                            .ThenByDescending(x => x.RevenueWinningPercent))
        {
            int winnerCount = 1;
            if (!tier.IsSingleTicketWinner)
            {
                winnerCount = (int)Math.Round(totalTicketsCount * (tier.TotalWinningPercent / 100));
            }

            for (int i = 1; i <= winnerCount; i++)
            {
                if (tickets.Count == 0 || currentTicketsCost == 0)
                {
                    break;
                }
                var winnerIndex = randomizer.GetRandomNumber(0, tickets.Count - 1);
                
                var prizeAmount = Math.Truncate(totalTicketsCost * (tier.RevenueWinningPercent / 100) / winnerCount * 100) / 100;
                if (prizeAmount > currentTicketsCost)
                {
                    prizeAmount = currentTicketsCost;
                    currentTicketsCost = 0;
                }
                else
                {
                    currentTicketsCost -= prizeAmount;
                }
                
                var winningTicket = new WinningTicket(tickets[winnerIndex].Ticket, tickets[winnerIndex].Player, tier, prizeAmount);
                tickets[winnerIndex].Player.AddRevenue(prizeAmount);
                winners.Add(winningTicket);
                tickets.RemoveAt(winnerIndex);
            }
        }

        return winners;
    }

}
