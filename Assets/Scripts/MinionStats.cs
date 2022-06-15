using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class MinionStats : NetworkBehaviour
{
    [SerializeField] internal MinionType minionType;
    [SyncVar(hook = nameof(OnReceiveHealth))] public int health;

    public HealthUI healthUI;


    private void OnReceiveHealth(int _old, int _new)
    {
        healthUI.UpdateHealth(_new, 100);
    }

    public UnityAction onDead;
    public void SetHealth(int _health) { health = _health; }
    public void DecreaseHealth(int _health)
    {
        if (health - _health <= 0)
        {
            onDead();
        }
        health -= _health;
    }

    [SerializeField] internal int damage;
    public void SetDamage(int _dmg) { damage = _dmg; }
    public void DecreaseDamage(int _dmg) { damage -= _dmg; }
}
