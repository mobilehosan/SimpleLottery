using ConsoleTables;
using SimpleLottery.Application;
using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Constants;

namespace SimpleLottery.Presentation.Services;

public interface ISimpleUIService
{
    void StartLottery();
}

class SimpleUIService(ILotteryRunner lotteryRunner, IInputOutputService inputOutputService) : ISimpleUIService
{
    private sealed record PlayersInfo(string PlayerName, decimal PlayerBalance, int NonWinningTicketsCount, int WinningTicketsCount);
    
    public void StartLottery()
    {
        Start();
        (string? userAnswer, bool isfinish) drawResult = ("", false);
        do
        {
            if (drawResult.isfinish)
            {
                Start();
            }
            drawResult = Draw();
        }
        while (drawResult.userAnswer?.ToUpper() == "Y");

        void Start()
        {
            lotteryRunner.StartNewLottery();
            PrintGreeting();
        }
    }

    private void Print(string text)
    {
        inputOutputService.PrintText(text);
    }

    private void PrintGreeting()
    {
        var lotteryOptions = lotteryRunner.GetLotteryOptions();
        Print($"Welcome to the Веdе lottery, {User.Name}!\n");
        Print("\n");
        Print($"* Your digital balance: ${lotteryOptions?.PlayerStartBalance}\n");
        Print($"* Ticket price: ${lotteryOptions?.TicketCost} each\n");
    }

    private (string? userAnswer, bool isfinish) Draw()
    {
        string fromScratchText = "";
        var isFinishResult = false;
        var userTicketsCount = GetUserTicketsCount();

        var lotteryResult = lotteryRunner.DoDraw(userTicketsCount);

        if (lotteryResult.LastDraw)
        {
            Print("None of the players has enought money to buy even a single ticket!\n");
            isFinishResult = true;
            fromScratchText = " from scratch";
        }
        else
        {
            var participantsCount = GetParticipantsCount(lotteryResult);

            Print("\n");
            Print($"{participantsCount} other CPU players also purchased tickets.\n");
            Print("\n");

            PrintWinners(lotteryResult);

            Print("Congratulations to the winners!\n");
            Print("\n");
            Print($"House Revenue: ${lotteryResult.HouseRevenue}\n");
            Print("\n");

            PrintGeneralStatistics(lotteryResult);
        }

        Print("\n");
        var userAnswer = inputOutputService.GetTextFromUser($"Do you want to play one more time{fromScratchText}? (Y/N)\n");

        return (userAnswer, isFinishResult);
    }

    private static int GetParticipantsCount(LotteryResult lotteryResult)
    {
        return lotteryResult.TicketsInfo
            .Count(x => x.Key.Name != User.Name &&
                        x.Value.Tickets.Count + x.Value.WinningTickets.Count > 0);
    }

    private int GetUserTicketsCount()
    {
        bool isSuccess;
        int result;
        do
        {
            Print("\n");
            var userTicketsCount = inputOutputService.GetTextFromUser($"How many tickets do you want to buy, {User.Name}?\n");
            isSuccess = int.TryParse(userTicketsCount, out result);
        }
        while (!isSuccess);

        return result;
    }

    private void PrintWinners(LotteryResult lotteryResult)
    {
        Print("Tickets Draw Results:\n");
        Print("\n");

        IEnumerable<string> tiers = GetTiersWithWinners(lotteryResult);
        foreach (string tier in tiers)
        {
            decimal ticketRevenue = GetTicketRevenue(lotteryResult, tier);

            var winners = GetWinners(lotteryResult, tier);

            bool multipleWinners = winners.Count() > 1;
            string playerText = multipleWinners ? "s" : "";
            string winText = multipleWinners ? "" : "s";
            string eachText = multipleWinners ? " each" : "";

            var playersIndices = string.Join(",", winners);

            Print($"* {tier}: Player{playerText} {playersIndices} win{winText} ${ticketRevenue}{eachText}!\n");
        }
        Print("\n");
    }

    private static IEnumerable<string> GetTiersWithWinners(LotteryResult lotteryResult)
    {
        return lotteryResult.TicketsInfo
            .SelectMany(x => x.Value.WinningTickets)
            .Select(x => x.Tier)
            .Distinct()
            .OrderByDescending(x => x.IsSingleTicketWinner)
            .ThenByDescending(x => x.RevenueWinningPercent)
            .Select(x => x.Name);
    }

    private static decimal GetTicketRevenue(LotteryResult lotteryResult, string tier)
    {
        return lotteryResult.TicketsInfo
            .SelectMany(x => x.Value.WinningTickets)
            .First(x => x.Tier.Name == tier)
            .Ammount;
    }

    private static IEnumerable<int> GetWinners(LotteryResult lotteryResult, string tier)
    {
        return lotteryResult.TicketsInfo
            .SelectMany(x => x.Value.WinningTickets)
            .Where(x => x.Tier.Name == tier)
            .Select(x => x.Player.Index)
            .Distinct();
    }

    private void PrintGeneralStatistics(LotteryResult lotteryResult) 
    {
        PrintPlayersStatistics(lotteryResult);
        PrintHouseStatistics(lotteryResult);
    }

    private void PrintPlayersStatistics(LotteryResult lotteryResult)
    {
        Print("===========================================================================");
        Print("\n");
        var str = ConsoleTable.From(GetPlayersInfo(lotteryResult)).ToString();
        Print(str);
        Print("\n");
        Print("===========================================================================");
        Print("\n");
    }

    private static List<PlayersInfo> GetPlayersInfo(LotteryResult lotteryResult)
    {
        return [.. lotteryResult.TicketsInfo.Select( x => new PlayersInfo(
            x.Key.Name, 
            x.Key.DigitalBalance, 
            x.Value.Tickets.Count, 
            x.Value.WinningTickets.Count))]; 
    }

    private void PrintHouseStatistics(LotteryResult lotteryResult)
    {
        Print("\n");
        Print($" House balance: {lotteryResult.House.Revenue}");
        Print("\n");
    }
}
