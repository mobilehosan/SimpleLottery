using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLottery.Application;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;
using SimpleLottery.Infrastructure;
using SimpleLottery.Presentation.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISimpleUIService, SimpleUIService>()
    .AddSingleton<IInputOutputService, InputOutputService>()
    .AddSingleton<IRandomizer, Randomizer>()
    .Configure<TiersOption>(configuration.GetSection(TiersOption.Tiers))
    .Configure<LotteryOptions>(configuration.GetSection(LotteryOptions.Lottery))
    .AddApplication()
    .BuildServiceProvider();

var simpleUIService = serviceProvider.GetRequiredService<ISimpleUIService>();

simpleUIService.StartLottery();