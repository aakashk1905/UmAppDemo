using Fusion;
using System.Collections.Generic;

public struct NetworkedPlayerRefList : INetworkStruct
{
    public const int MAX_PLAYERS = 100;
    [Networked, Capacity(MAX_PLAYERS)]
    public NetworkArray<PlayerRef> Players { get; }

    [Networked]
    public int Count { get; set; }

    public void Add(PlayerRef playerRef)
    {
        if (Count < MAX_PLAYERS)
        {
            Players.Set(Count, playerRef);
            Count++;
        }
    }

    public void Remove(PlayerRef playerRef)
    {
        for (int i = 0; i < Count; i++)
        {
            if (Players.Get(i) == playerRef)
            {
                
                for (int j = i; j < Count - 1; j++)
                {
                    Players.Set(j, Players.Get(j + 1));
                }
                Count--;
                break;
            }
        }
    }

    public bool Contains(PlayerRef playerRef)
    {
        for (int i = 0; i < Count; i++)
        {
            if (Players.Get(i) == playerRef)
            {
                return true;
            }
        }
        return false;
    }


    public List<PlayerRef> ToList()
    {
        var list = new List<PlayerRef>(Count);
        for (int i = 0; i < Count; i++)
        {
            list.Add(Players.Get(i));
        }
        return list;
    }
}