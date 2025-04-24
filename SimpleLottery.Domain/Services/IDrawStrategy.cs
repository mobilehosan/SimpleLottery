using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Entities;
using SimpleLottery.Domain.Options;

namespace SimpleLottery.Domain.Services;

interface IDrawStrategy
{
    DrawResult Draw(List<(Player Player, Ticket Ticket)> tickets, TiersOption tiers);
}
