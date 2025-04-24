using SimpleLottery.Domain.Entities;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;
using SimpleLottery.Domain.Services.DrawStrategies;

namespace SimpleLottery.Domain.Aggregates;

public class Lottery
{
    internal TiersOption Tiers { get; init; }
    internal House House { get; init; }

    private List<(Player Player, Ticket Ticket)> _tickets = [];
    
    private readonly Players _players;

    private readonly LotteryOptions _lotteryOptions;

    private readonly IDrawStrategy _drawStrategy;

    private readonly IRandomizer _randomizer;

    private const string DefaultHouseName = "Casino";

    public Lottery(TiersOption tiers, LotteryOptions lotteryOptions, IRandomizer randomizer)
    {
        _lotteryOptions = lotteryOptions;
        _randomizer = randomizer;

        Tiers = tiers;
        House = House.CreateHouse(DefaultHouseName);
        
        var totalPlayersCount = _randomizer.GetRandomNumber(_lotteryOptions.TotalPlayersCountMin, _lotteryOptions.TotalPlayersCountMax);
        
        _players = Players.GeneratePlayers(totalPlayersCount, _lotteryOptions.PlayerStartBalance);
        _drawStrategy = new SimpleDrawStrategy(_randomizer);
    }

    public LotteryResult Draw(int userTicketsCount) 
    {
        SetUserTicketsCount(userTicketsCount);
        _tickets = GetGeneratedTickets();
        
        DrawResult result = _drawStrategy.Draw(_tickets, Tiers);
        
        House.AddRevenue(result.HouseRevenue);

        Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> ticketsInfo = GetTicketsInfo(result.WinningTickets);

        return LotteryResult.CreateLotteryResult(House, result.HouseRevenue, ticketsInfo);
    }

    private void SetUserTicketsCount(int userTicketsCount)
    {
        foreach (var player in _players.PlayersList.Where(x => x.IsUser))
        {
            player.UserTicketsCount = userTicketsCount;
        }
    }

    private List<(Player Player, Ticket Ticket)> GetGeneratedTickets()
    {
        List<(Player Player, Ticket Ticket)> tickets = [];
        foreach (var player in _players.PlayersList)
        {
            var ticketsCount = player.UserTicketsCount ?? _randomizer.GetRandomNumber(_lotteryOptions.PlayersTicketsCountMin, _lotteryOptions.PlayersTicketsCountMax);
            for (int i = 1; i <= ticketsCount; i++)
            {
                bool isSuccess = player.TryWithdrawTicketCost(_lotteryOptions.TicketCost);
                if (!isSuccess) 
                {
                    break;
                }
                var ticket = Ticket.CreateTicket(_lotteryOptions.TicketCost);
                tickets.Add((player, ticket));
            }
        }

        return tickets;
    }

    private Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> GetTicketsInfo(IReadOnlyList<WinningTicket> winningTickets) 
    {
        Dictionary<Player, (List<Ticket> Tickets, List<WinningTicket> WinningTickets)> result = [];
        foreach (var player in _players.PlayersList) 
        {
            List<Ticket> currentTicketList = [.. _tickets.Where(x => x.Player == player).Select(x => x.Ticket)];
            List<WinningTicket> currentWinningTickets = [.. winningTickets.Where(x => x.Player == player)];

            result.Add(player, (currentTicketList, currentWinningTickets));
        }

        return result;
    }
}
