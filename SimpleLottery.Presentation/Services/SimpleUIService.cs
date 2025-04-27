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
    public void StartLottery() 
    {
        lotteryRunner.StartNewLottery();

        PrintGreeting();

        string? userAnswer;
        do
        {
            userAnswer = Draw();
        }
        while (userAnswer?.ToUpper() == "Y");
    }

    private void Print(string text) 
    {
        inputOutputService.PrintText(text);
    }

    private void PrintGreeting() 
    {
        var lotteryOptions = lotteryRunner.GetLotteryOptions();
        Print($"Welcome to the Bede lottery, {User.Name}!\n");
        Print("\n");
        Print($"* Your digital balance: ${lotteryOptions?.PlayerStartBalance}\n");
        Print($"* Ticket price: ${lotteryOptions?.TicketCost} each\n");
    }

    private string? Draw()
    {
        var userTicketsCount = GetUserTicketsCount();
        
        var lotteryResult = lotteryRunner.DoDraw(userTicketsCount);
        
        var participantsCount = GetParticipantsCount(lotteryResult);

        Print("\n");
        Print($"{participantsCount} other CPU players also purchased tickets.\n");
        Print("\n");
    
        PrintWinners(lotteryResult);
    
        Print("Congratulations to the winners!\n");
        Print("\n");
        Print($"House Revenue: ${lotteryResult.HouseRevenue}\n");
        Print("\n");
    
        var userAnswer = inputOutputService.GetTextFromUser("Do you want to play one more time? (Y/N)\n");
    
        return userAnswer;
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
}
