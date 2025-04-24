using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLottery.Application;
using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;
using SimpleLottery.Infrastructure;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IInputOutputService, InputOutputService>()
    .AddSingleton<IRandomizer, Randomizer>()
    .Configure<TiersOption>(configuration.GetSection(TiersOption.Tiers))
    .Configure<LotteryOptions>(configuration.GetSection(LotteryOptions.Lottery))
    .AddApplication()
    .BuildServiceProvider();

var lotteryRunner = serviceProvider.GetRequiredService<ILotteryRunner>();
var inputOutputService = serviceProvider.GetRequiredService<IInputOutputService>();

lotteryRunner.StartNewLottery();

var lotteryOptions = lotteryRunner.GetLotteryOptions();
inputOutputService.PrintText("Welcome to the Bede lottery, Player 1!\n");
inputOutputService.PrintText("\n");
inputOutputService.PrintText($"* Your digital balance: ${lotteryOptions?.PlayerStartBalance}\n");
inputOutputService.PrintText($"* Ticket price: ${lotteryOptions?.TicketCost} each\n");

string? userAnswer;
do
{
    userAnswer = Draw();
}
while (userAnswer?.ToUpper() == "Y");

string ? Draw() 
{
    var userTicketsCount = GetUserTicketsCount();
    var lotteryResult = lotteryRunner.DoDraw(userTicketsCount);
    var countPlayersWithTickets = lotteryResult.TicketsInfo
        .Count(x => x.Key.Name != "Player 1" && 
                    x.Value.Tickets.Count + x.Value.WinningTickets.Count > 0);

    inputOutputService.PrintText("\n");
    inputOutputService.PrintText($"{countPlayersWithTickets} other CPU players also purchased tickets.\n");
    inputOutputService.PrintText("\n");
    
    PrintWinners(lotteryResult);
    
    inputOutputService.PrintText("Congratulations to the winners!\n");
    inputOutputService.PrintText("\n");
    inputOutputService.PrintText($"House Revenue: ${lotteryResult.HouseRevenue}\n");
    inputOutputService.PrintText("\n");

    var userAnswer = inputOutputService.GetTextFromUser("Do you want to play one more time? (Y/N)\n");
    
    return userAnswer;
}

int GetUserTicketsCount() 
{
    bool isSuccess = false;
    int result = 0;
    do
    {
        inputOutputService.PrintText("\n");
        var userTicketsCount = inputOutputService.GetTextFromUser("How many tickets do you want to buy, Player 1?\n");
        isSuccess = int.TryParse(userTicketsCount, out result);
    }
    while (!isSuccess);

    return result;
}

void PrintWinners(LotteryResult lotteryResult) 
{
    inputOutputService.PrintText("Tickets Draw Results:\n");
    inputOutputService.PrintText("\n");

    List<TierOption> tiers = [.. lotteryResult.TicketsInfo.SelectMany(x => x.Value.WinningTickets).Select(x => x.Tier).Distinct()];
    foreach (var tier in tiers.OrderByDescending(x => x.IsSingleTicketWinner)
                              .ThenByDescending(x => x.RevenueWinningPercent)
                              .Select(x => x.Name)) 
    {
        decimal ticketRevenue = lotteryResult.TicketsInfo
            .SelectMany(x => x.Value.WinningTickets)
            .First(x => x.Tier.Name == tier)
            .Ammount;

        var winners = lotteryResult.TicketsInfo
                .SelectMany(x => x.Value.WinningTickets)
                .Where(x => x.Tier.Name == tier)
                .Select(x => x.Player.Index)
                .Distinct();

        string playerText = "";
        string winText = "";
        string eachText = "";

        if (winners.Count() > 1)
        {
            playerText = "s";
            winText = "";
            eachText = " each";
        }
        else 
        {
            playerText = "";
            winText = "s";
            eachText = "";
        }

        var playersIndices = string.Join(",", winners);

        inputOutputService.PrintText($"* {tier}: Player{playerText} {playersIndices} win{winText} ${ticketRevenue}{eachText}!\n");
    }
    inputOutputService.PrintText("\n");
}
