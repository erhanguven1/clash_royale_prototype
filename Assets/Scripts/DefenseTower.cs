using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class DefenseTower : NetworkBehaviour
{
    [SyncVar] public int owner;
    [SerializeField] [SyncVar] internal int maxHealth;
    [SerializeField] [SyncVar(hook = nameof(ReceiveHealthFromServer))] internal int health;
    [SerializeField] internal HealthUI healthUI;

    public Minion targetMinion;

    internal void DecreaseHealth(int _dmg)
    {
        health -= _dmg;
        if (health <= 0)
        {
            health = 0;
            TowerManager.Instance.OnTowerDestroyed(this);
            Destroy(gameObject, .1f);
        }
    }

    public void ReceiveHealthFromServer(int _old, int _new)
    {
        healthUI.UpdateHealth(_new, maxHealth > 0 ? maxHealth : 1);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        maxHealth = 100;
        health = 100;
    }

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

    [Server]
    internal void TakeDamage(int _dmg)
    {
        DecreaseHealth(_dmg);
    }

    internal bool IsEnemyWith(Minion _minion)
    {
        return owner != _minion.owner;
    }

    [Server]
    public void Attack(Minion attackTo)
    {
        attackTo.TakeDamage(80);
    }
}
