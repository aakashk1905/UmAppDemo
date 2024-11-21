using Fusion;

public partial class PlayerController : NetworkBehaviour
{
    private ChangeDetector _changeDetector;

    private void InitializeNetworking()
    {
        _changeDetector = new ChangeDetector();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_RequestSetPlayerInfo(int id, string nickname, string myId)
    {
        RPC_SetPlayerInfo(Object.InputAuthority, id, nickname, myId);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_UpdatePlayerSprite(bool isAlone)
    {
        _player.sprite = isAlone ? _sprites[0] : _sprites[1];
    }
}