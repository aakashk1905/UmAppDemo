
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct DSUEntry : INetworkStruct
{
    public PlayerRef Parent;
    public int Rank;
}

public class NetworkedDSU : NetworkBehaviour
{
    private static NetworkedDSU _instance;
    public static NetworkedDSU Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkedDSU>();
            }
            return _instance;
        }
    }

    [Networked, Capacity(100)] private NetworkDictionary<PlayerRef, DSUEntry> DsuData { get; }

    public override void Spawned()
    {
        _instance = this;
    }

    public void MakeSet(PlayerRef player)
    {
        if (Object.HasStateAuthority && !DsuData.ContainsKey(player))
        {
            DsuData.Set(player, new DSUEntry { Parent = player, Rank = 0 });
        }
    }
    public bool AreConnected(PlayerRef x, PlayerRef y)
    {
        return Find(x) == Find(y);
    }

    public PlayerRef Find(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return PlayerRef.None;

        if (!DsuData.TryGet(player, out var entry))
        {
            MakeSet(player);
            return player;
        }

        if (entry.Parent != player)
        {
            var root = Find(entry.Parent);
            entry.Parent = root;
            DsuData.Set(player, entry);
            return root;
        }
        return player;
    }

    public void Union(PlayerRef player1, PlayerRef player2)
    {
        if (!Object.HasStateAuthority) return;

        PlayerRef root1 = Find(player1);
        PlayerRef root2 = Find(player2);

        if (root1 == root2) return;

        if (!DsuData.TryGet(root1, out var entry1) || !DsuData.TryGet(root2, out var entry2))
        {
            Debug.LogError("Failed to get DSU entries");
            return;
        }

        if (entry1.Rank < entry2.Rank)
        {
            DsuData.Set(root1, new DSUEntry { Parent = root2, Rank = entry1.Rank });
        }
        else if (entry1.Rank > entry2.Rank)
        {
            DsuData.Set(root2, new DSUEntry { Parent = root1, Rank = entry2.Rank });
        }
        else
        {
            DsuData.Set(root2, new DSUEntry { Parent = root1, Rank = entry2.Rank + 1 });
        }
    }

    public void DisconnectPlayer(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return;
        ReevaluateGroups();
    }

    public void ReevaluateGroups()
    {
        Debug.LogError("Re-evaluating groups using DFS for all players");

        HashSet<PlayerRef> visited = new HashSet<PlayerRef>();
        List<List<PlayerRef>> groups = new List<List<PlayerRef>>();

        // Iterate through all players in DsuData
        foreach (var kvp in DsuData)
        {
            PlayerRef player = kvp.Key;
            if (!visited.Contains(player))
            {
                List<PlayerRef> group = new List<PlayerRef>();
                DFS(player, visited, group);
                Debug.LogError("Logging group ===" +string.Join(", ", group));
               if(group.Count > 1) {
                    groups.Add(group);
                }
            }
        }

        // Clear and recreate the DSU structure based on the identified groups
        DsuData.Clear();
        foreach (var group in groups)
        {
            if (group.Count > 0)
            {
                PlayerRef root = group[0];
                MakeSet(root);
                for (int i = 1; i < group.Count; i++)
                {
                    MakeSet(group[i]);
                    Union(root, group[i]);
                }
            }
        }

        Debug.LogError("Groups after re-evaluation:");
        foreach (var group in groups)
        {
            Debug.LogError($"Group: {string.Join(", ", group)}");
        }
    }

    private void DFS(PlayerRef player, HashSet<PlayerRef> visited, List<PlayerRef> group)
    {
        visited.Add(player);
        group.Add(player);
        PlayerController playerc = Runner.GetPlayerObject(player).GetComponent<PlayerController>();
            foreach(var neighbor in playerc.neighbours)
            {
                if (!visited.Contains(neighbor))
                {
                    DFS(neighbor, visited, group);
                }
            } 
    }


    public Dictionary<PlayerRef, List<PlayerRef>> GetCurrentGroups()
    {
        var groups = new Dictionary<PlayerRef, List<PlayerRef>>();

        // Iterate over the NetworkDictionary using its enumerator
        foreach (var kvp in DsuData)
        {
            PlayerRef player = kvp.Key;
            PlayerRef root = Find(player);
            if (!groups.ContainsKey(root))
            {
                groups[root] = new List<PlayerRef>();
            }
            groups[root].Add(player);
           
        }

        Debug.LogError("Current groups:");
        foreach (var group in groups)
        {
            Debug.LogError($"Root: {group.Key}, Members: {string.Join(", ", group.Value)}");
        }

        return groups;
    }
}
