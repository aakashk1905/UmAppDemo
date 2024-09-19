using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerController : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnChannelChanged))]
    public NetworkString<_128> _channelName { get; set; } = "";
    [Networked, OnChangedRender(nameof(OnRoomChanged))]
    public NetworkBool IsInRoom { get; set; } = false;
    public string myChannel = "";
    public string prevChannel = "";
    public string _token = "";
    public bool isInChannel = false;
    public bool IamBridge = false;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();

    private void InitializeChannelManager()
    {
        _channelName = "";
        tokens = new Dictionary<string, string>();
    }

    public void OnChannelChanged()
    {
        if (Object.HasInputAuthority)
        {
            prevChannel = myChannel;
            myChannel = _channelName.Value;

            if (!string.IsNullOrEmpty(_channelName.Value) && _channelName.Value != "dummy" && !isInChannel)
            {
                _agoraManager.AddPlayerToChannel(_channelName.Value, this);
                if (Object.HasStateAuthority)
                {
                    networkTableManager.Rpc_UpdateNetworkTable("add", _channelName.Value, this.Object.InputAuthority);
                }
                else
                {
                    Rpc_RequestTableUpdate("add", _channelName.Value, this.Object.InputAuthority);
                }
            }
        }
    }

    public void OnRoomChanged()
    {
        UpdateSprite();
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_UpdateChannelsAfterUnion(PlayerController otherPlayer)
    {
        if (string.IsNullOrEmpty(_channelName.Value) && string.IsNullOrEmpty(otherPlayer._channelName.Value))
        {
            string newChannel = _agoraManager.GenerateChannelName("" + _playerID);
            Rpc_SetChannelName(newChannel);
        }
        else if (string.IsNullOrEmpty(_channelName.Value) && !string.IsNullOrEmpty(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(otherPlayer._channelName.Value);
        }
        else if (!string.IsNullOrEmpty(_channelName.Value) && !string.IsNullOrEmpty(otherPlayer._channelName.Value) &&
            _channelName.Value != otherPlayer._channelName.Value)
        {
            RpcMergeChannels(_channelName.Value, otherPlayer._channelName.Value);
        }

        _player.sprite = _sprites[1];
    }

    private void ReorganizeGroupAfterExit(PlayerController exitingPlayer)
    {
        string currentChannel = _channelName.Value;
        _networkedDSU.DisconnectPlayer(exitingPlayer.Object.InputAuthority);

        if (neighbours.Count == 0)
        {
            HandleChannelChange(this, currentChannel, "");
            return;
        }

        var groups = _networkedDSU.GetCurrentGroups();

        if (groups.Count == 1)
        {
            return;
        }

        var largestGroup = groups.Values.OrderByDescending(g => g.Count).First();
        foreach (var group in groups.Values)
        {
            if (group == largestGroup)
            {
                continue;
            }

            string newChannel = "Split" + group.First();
            foreach (var playerRef in group)
            {
                PlayerController playerController = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
                HandleChannelChange(playerController, currentChannel, newChannel);
            }
        }

        UpdatePlayerSprite();
    }

    private void HandleChannelChange(PlayerController player, string oldChannel, string newChannel)
    {
        networkTableManager.Rpc_UpdateNetworkTable("remove", oldChannel, player.Object.InputAuthority);
        if (!string.IsNullOrEmpty(newChannel))
            networkTableManager.Rpc_UpdateNetworkTable("add", newChannel, player.Object.InputAuthority);
        player.Rpc_LeaveChannel(player.Object.InputAuthority, oldChannel);
        player.Rpc_SetIsInChannel(false);
        player.Rpc_SetChannelName(newChannel);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_LeaveChannel(PlayerRef playerRef, string channel)
    {
        if (Object.HasInputAuthority)
        {
            PlayerController player = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
            _agoraManager.LeaveChannel(player, channel);
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_SetChannelName(string name)
    {
        _channelName = name;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_SetIsInChannel(bool value)
    {
        isInChannel = value;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcMergeChannels(string channel1, string channel2)
    {
        Debug.LogError("Merging channels: " + channel1 + " and " + channel2);
        Dictionary<string, List<PlayerRef>> networkTable = networkTableManager.GetNetworkTableAsDictionary();

        if (!networkTable.ContainsKey(channel1) || !networkTable.ContainsKey(channel2)) return;

        List<PlayerRef> channel1Players = networkTable[channel1];
        List<PlayerRef> channel2Players = networkTable[channel2];
        string targetChannel = channel1Players.Count >= channel2Players.Count ? channel1 : channel2;
        string sourceChannel = channel1Players.Count >= channel2Players.Count ? channel2 : channel1;

        Debug.LogError($"Target channel: {targetChannel}, Source channel: {sourceChannel}");

        List<PlayerRef> playersToMove = networkTable[sourceChannel];

        foreach (var playerRef in playersToMove)
        {
            networkTableManager.Rpc_UpdateNetworkTable("remove", sourceChannel, playerRef);
            PlayerController player = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
            player.Rpc_LeaveChannel(playerRef, sourceChannel);
            player.Rpc_SetIsInChannel(false);
            player.Rpc_SetChannelName(targetChannel);
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void Rpc_RequestTableUpdate(string action, string channelName, PlayerRef player)
    {
        networkTableManager.Rpc_UpdateNetworkTable(action, channelName, player);
    }

    public string GetChannelName() { return _channelName.Value; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_UpdateIsInRoom(bool value)
    {
        IsInRoom = value;
        OnRoomChanged();
    }
    public string GetToken() { return _token; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcSetToken(string newToken)
    {
        _token = newToken;
    }
}