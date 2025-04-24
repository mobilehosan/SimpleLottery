using Microsoft.Extensions.DependencyInjection;

namespace SimpleLottery.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ILotteryRunner, LotteryRunner>();

        return services;
    }
}
