using FluentAssertions;
using Moq;
using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Entities;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;
using Xunit;

namespace SimpleLottery.Domain.Tests.Aggregates;

public class LotteryTest
{
    private readonly Lottery _sut;
    private readonly Mock<IRandomizer> _randomizer = new();

    public LotteryTest() 
    {
        List<TierOption> tiersList = [];
        tiersList.Add(new TierOption() { Name = "Grand Prize", IsSingleTicketWinner = true, TotalWinningPercent = 0, RevenueWinningPercent = 50 });
        tiersList.Add(new TierOption() { Name = "Second Tier", IsSingleTicketWinner = false, TotalWinningPercent = 10, RevenueWinningPercent = 30 });
        tiersList.Add(new TierOption() { Name = "Third Tier", IsSingleTicketWinner = false, TotalWinningPercent = 20, RevenueWinningPercent = 10 });

        LotteryOptions lotteryOptions = new()
        {
            PlayerStartBalance = 10,
            PlayersTicketsCountMax = 10,
            PlayersTicketsCountMin = 1,
            TicketCost = 1,
            TotalPlayersCountMax = 15,
            TotalPlayersCountMin = 10
        };

        TiersOption tiers = new() { TiersList = tiersList };
        
        _randomizer.SetupSequence(x => x.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(12) // players count
            .Returns(10) // tickets count for Player 2
            .Returns(10) // tickets count for Player 3
            .Returns(10) // tickets count for Player 4
            .Returns(10) // tickets count for Player 5
            .Returns(10) // tickets count for Player 6
            .Returns(10) // tickets count for Player 7
            .Returns(10) // tickets count for Player 8
            .Returns(10) // tickets count for Player 9
            .Returns(10) // tickets count for Player 10
            .Returns(10) // tickets count for Player 11
            .Returns(10) // tickets count for Player 12
            .Returns(114)// Grand Prize = last ticket for Player 12
            .Returns(0) // Second Tier = 1 ticket for Player 1
            .Returns(0) // Second Tier = 2 ticket for Player 1
            .Returns(0) // Second Tier = 3 ticket for Player 1
            .Returns(0) // Second Tier = 4 ticket for Player 1
            .Returns(0) // Second Tier = 5 ticket for Player 1
            .Returns(0) // Second Tier = 1 ticket for Player 2
            .Returns(0) // Second Tier = 2 ticket for Player 2
            .Returns(0) // Second Tier = 3 ticket for Player 2
            .Returns(0) // Second Tier = 4 ticket for Player 2
            .Returns(0) // Second Tier = 5 ticket for Player 2
            .Returns(0) // Second Tier = 6 ticket for Player 2
            .Returns(0) // Second Tier = 7 ticket for Player 2
            .Returns(0) // ...
            ;

        _sut = new Lottery(tiers, lotteryOptions, _randomizer.Object);
    }

    [Fact]
    public void Draw_WhenInvokedWithDefaultOptionsAndFiveUsersTickets_ShouldReturnWinningTickets() 
    {
        
        LotteryResult lotteryResult = _sut.Draw(5); // tickets count for Player 1 = 5
        // all tickets count = 115 
        // Grand Prize = 1 ticket ,          $57.50
        // Second Tier = 10% of 115 = 12,    $ 2.87   left $0.06 for Casino because of roundings
        // Third Tier = 20% of 115 = 23      $ 0.50 

        lotteryResult.TicketsInfo.Keys.Count().Should().Be(12);
        var player1 = Player.CreatePlayer("Player 1", 1, 0);
        lotteryResult.TicketsInfo[player1].WinningTickets.Count.Should().Be(5);
        lotteryResult.TicketsInfo[player1].WinningTickets[0].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player1].WinningTickets[1].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player1].WinningTickets[2].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player1].WinningTickets[3].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player1].WinningTickets[4].Ammount.Should().Be(2.87m);

        var player2 = Player.CreatePlayer("Player 2", 2, 0);
        lotteryResult.TicketsInfo[player2].WinningTickets.Count.Should().Be(10);
        lotteryResult.TicketsInfo[player2].WinningTickets[0].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[1].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[2].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[3].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[4].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[5].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[6].Ammount.Should().Be(2.87m);
        lotteryResult.TicketsInfo[player2].WinningTickets[7].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player2].WinningTickets[8].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player2].WinningTickets[9].Ammount.Should().Be(0.5m);

        var player3 = Player.CreatePlayer("Player 3", 3, 0);
        lotteryResult.TicketsInfo[player3].WinningTickets.Count.Should().Be(10);
        lotteryResult.TicketsInfo[player3].WinningTickets[0].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[1].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[2].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[3].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[4].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[5].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[6].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[7].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[8].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player3].WinningTickets[9].Ammount.Should().Be(0.5m);

        var player4 = Player.CreatePlayer("Player 4", 4, 0);
        lotteryResult.TicketsInfo[player4].WinningTickets.Count.Should().Be(10);
        lotteryResult.TicketsInfo[player4].WinningTickets[0].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[1].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[2].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[3].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[4].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[5].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[6].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[7].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[8].Ammount.Should().Be(0.5m);
        lotteryResult.TicketsInfo[player4].WinningTickets[9].Ammount.Should().Be(0.5m);

        var player5 = Player.CreatePlayer("Player 5", 5, 0);
        lotteryResult.TicketsInfo[player5].Tickets.Count.Should().Be(10);
        lotteryResult.TicketsInfo[player5].WinningTickets.Count.Should().Be(0);

        var player12 = Player.CreatePlayer("Player 12", 12, 0);
        lotteryResult.TicketsInfo[player12].Tickets.Count.Should().Be(9);
        lotteryResult.TicketsInfo[player12].WinningTickets.Count.Should().Be(1);
        lotteryResult.TicketsInfo[player12].WinningTickets[0].Ammount.Should().Be(57.5m);

        lotteryResult.House.Revenue.Should().Be(11.56m);
    }
}
