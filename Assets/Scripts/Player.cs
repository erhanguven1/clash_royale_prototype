using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] internal int id;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [Server]
    public void SetID(int _id)
    {
        id = _id;
        SetIDOnClients(id);
    }

    [ClientRpc]
    void SetIDOnClients(int _id)
    {
        id = _id;
    }

    [ClientRpc]
    public void SetPlayer()
    {
        if (hasAuthority)
        {
            MatchManager.Instance.myPlayer = this;
            ArenaManager.Instance.SetArenaRotation(id);
            print("has authority!");
        }
        else
        {
            MatchManager.Instance.opponentPlayer = this;
            print("no authority!");
        }
    }
}
