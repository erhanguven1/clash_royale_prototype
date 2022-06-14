using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[System.Serializable]
internal class DefenseTowerStats
{

    [SerializeField] [SyncVar] internal int health;

    internal void DecreaseHealth(int _dmg)
    {
        health -= _dmg;
        if (health <= 0)
        {

        }
    }
}

public class DefenseTower : NetworkBehaviour
{
    [SyncVar] public int owner;
    [SerializeField] internal DefenseTowerStats stats;

    public void SetTowerPosition(int towerTransformIndex)
    {
        transform.parent = ArenaManager.Instance.towerTransforms[towerTransformIndex];
        transform.localPosition = Vector3.zero;

        SYNCTowerPosition(towerTransformIndex, owner);
    }

    [ClientRpc]
    void SYNCTowerPosition(int towerTransformIndex, int _owner)
    {
        if (MatchManager.Instance.myPlayer.id == 1)
        {
            transform.parent = ArenaManager.Instance.towerTransforms[(towerTransformIndex + 2) % 4];
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.parent = ArenaManager.Instance.towerTransforms[towerTransformIndex];
            transform.localPosition = Vector3.zero;
        }
    }

    internal void SetStats(DefenseTowerStats _stats)
    {
        stats = _stats;
    }

    [Server]
    internal void TakeDamage(int _dmg)
    {
        stats.DecreaseHealth(_dmg);
    }

    internal bool IsEnemyWith(Minion _minion)
    {
        return owner != _minion.owner;
    }
}
