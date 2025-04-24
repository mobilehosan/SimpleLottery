using SimpleLottery.Domain.Entities;

namespace SimpleLottery.Domain.Aggregates;

public class Players
{
    private readonly List<Player> _playersList;

    public IReadOnlyList<Player> PlayersList => _playersList;

    private Players(List<Player> playersList)
    {
        _playersList = playersList;
    }

    public static Players GeneratePlayers(int totalPlayersCount, decimal playerStartBalance) 
    {
        List<Player> players = [];

        for (int i = 1; i <= totalPlayersCount; i++) 
        { 
            var player = Player.CreatePlayer($"Player {i}", i, playerStartBalance, i == 1);
            players.Add(player);
        }
        return new Players(players);
    }
}
