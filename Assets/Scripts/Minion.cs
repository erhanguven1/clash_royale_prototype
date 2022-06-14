using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;
using Mirror;

[System.Serializable]
public class MinionStats
{
    [SerializeField] MinionType minionType;
    [SerializeField] internal int health;

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

    [SerializeField] internal int attackInterval;

    internal MinionStats(MinionData minionData)
    {
        this.minionType = minionData.minionType;
        this.health = minionData.health;
        this.damage = minionData.damage;
        this.attackInterval = minionData.attackInterval;
    }
}

public enum AttackTarget { Minion, Tower }

public abstract class Minion : NetworkBehaviour
{
    [SerializeField] internal int owner;

    [SerializeField] internal MinionStats minionStats;
    [SerializeField] internal MinionAnimationController animationController;

    private float attackCountdown;
    [SerializeField] internal bool isAttacking;
    [SerializeField] internal AttackTarget attackTarget;

    internal List<Minion> targetMinions = new List<Minion>();
    [SerializeField] Minion targetMinion;
    [SerializeField] DefenseTower targetTower;

    private void Start()
    {
        if (GetComponent<NetworkIdentity>().isClient)
        {
            Destroy(GetComponent<Rigidbody>());
            transform.GetChild(0).eulerAngles = new Vector3(0, owner * 180, 0);
        }
    }

    public void SetMinionData(MinionData _minionData)
    {
        minionStats = new MinionStats(_minionData);
        attackCountdown = minionStats.attackInterval;

        minionStats.onDead += OnDead;

        switch (_minionData.minionType)
        {
            case MinionType.Volva:
                gameObject.AddComponent<SphereCollider>().enabled = false;
                gameObject.GetComponent<SphereCollider>().isTrigger = true;
                gameObject.GetComponent<SphereCollider>().radius = 3;
                gameObject.GetComponent<SphereCollider>().enabled = true;
                break;
            case MinionType.Berserker:
                gameObject.AddComponent<SphereCollider>().enabled = false;
                gameObject.GetComponent<SphereCollider>().isTrigger = true;
                gameObject.GetComponent<SphereCollider>().radius = 1;
                gameObject.GetComponent<SphereCollider>().enabled = true;
                break;
            default:
                break;
        }
    }

    private void OnDead()
    {
        MinionManager.Instance.OnMinionDead(this);

        minionStats.onDead -= OnDead;
        gameObject.SetActive(false);
        Destroy(gameObject, .1f);
    }

    public void AnotherMinionDead(Minion minion)
    {
        if (targetMinions.Contains(minion))
        {
            targetMinions.Remove(minion);

            if (targetMinion == minion)
            {
                isAttacking = false;
                animationController.StartWalking();
                ChooseTargetMinion();
            }
        }
    }

    internal abstract void AttackMinion(Minion attackTo);
    internal abstract void AttackTower(DefenseTower attackTo);

    [Server]
    public void TakeDamage(int dmg)
    {
        minionStats.DecreaseHealth(dmg);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (GetComponent<NetworkIdentity>().isClient)
        {
            return;
        }

        var otherMinion = other.GetComponent<Minion>();

        if (otherMinion != null)
        {
            print("encounter with minion!");

            if (otherMinion.IsEnemyWith(this))
            {
                targetMinions.Add(otherMinion);
                ChooseTargetMinion();

                if (!isAttacking)
                {
                    isAttacking = true;
                    animationController.StartAttacking();

                    attackTarget = AttackTarget.Minion;
                }
            }

            return;
        }

        var tower = other.GetComponent<DefenseTower>();

        //If there is no target minion in queue, start attacking to the tower
        if (targetMinions.Count == 0 && tower != null)
        {
            if (tower.IsEnemyWith(this))
            {
                targetTower = tower;

                if (!isAttacking)
                {
                    isAttacking = true;
                    attackTarget = AttackTarget.Tower;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GetComponent<NetworkIdentity>().isClient)
        {
            return;
        }

        var otherMinion = other.GetComponent<Minion>();

        if (otherMinion != null)
        {
            if (otherMinion.IsEnemyWith(this))
            {
                targetMinions.Remove(other.GetComponent<Minion>());
                ChooseTargetMinion();
            }
        }

        //If there is no target minion in queue, start attacking to the tower
        if (targetMinions.Count == 0 && targetTower != null)
        {
            if (targetTower.IsEnemyWith(this))
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    animationController.StartAttacking();

                    attackTarget = AttackTarget.Tower;
                }
            }
        }
    }

    void ChooseTargetMinion()
    {
        targetMinion = null;

        if (targetMinions.Count == 0)
        {
            isAttacking = false;
            animationController.StartWalking();

        }
        else
        {
            foreach (var item in targetMinions)
            {
                if (item == null)
                {
                    print("null");
                }
            }

            var orderedTargets = targetMinions.OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

            targetMinion = orderedTargets.First();
        }
    }

    internal bool IsEnemyWith(Minion _minion)
    {
        //If owners are different, return true.
        return owner != _minion.owner;
    }

    private void Update()
    {
        if (GetComponent<NetworkIdentity>().isClient)
        {
            return;
        }

        if (isAttacking)
        {
            attackCountdown -= Time.deltaTime;

            if (attackCountdown <= 0)
            {
                attackCountdown = minionStats.attackInterval;
                if (attackTarget == AttackTarget.Minion)
                {
                    AttackMinion(targetMinion);
                }
                else
                {
                    AttackTower(targetTower);
                }
            }
        }
        else
        {
            Walk();
        }
    }

    [Server]
    void Walk()
    {
        transform.position += this.ArenaDirection() * Time.deltaTime * .5f;
    }
    private void OnDrawGizmos()
    {
        if (targetMinion == null)
        {
            return;
        }
        Gizmos.DrawLine(transform.position, targetMinion.transform.position);
    }
}
