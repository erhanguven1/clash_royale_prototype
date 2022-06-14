using Mirror;
using UnityEngine;

public class MatchManager : InstancableNB<MatchManager>
{
    //In server, myPlayer and opponentPlayer variables will be NULL
    [SerializeField] internal Player myPlayer, opponentPlayer;

    [Command(requiresAuthority = false)]
    public void RequestSpawnMinion(int _owner, MinionType _minionType, Vector3 _spawnPosition)
    {
        SpawnMinionOnClient(MinionSpawner.Instance.SpawnMinion(_owner, _minionType, _spawnPosition), _owner, _minionType, _spawnPosition);
    }

    [ClientRpc]
    void SpawnMinionOnClient(NetworkIdentity identity, int _owner, MinionType _minionType, Vector3 _spawnPosition)
    {
        MinionFactory.Instance.InitMinionOnClient(identity, _minionType, _spawnPosition, _owner);
    }
}
