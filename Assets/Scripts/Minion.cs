﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;
using Mirror;
using Unity.VisualScripting;

public enum AttackTarget { Minion, Tower }

public class Minion : NetworkBehaviour
{
    [SerializeField] internal int attackInterval;

    [SerializeField] internal int owner;

    [SerializeField] internal MinionAnimationController animationController;

    private float attackCountdown;
    [SerializeField] internal bool isAttacking;
    [SerializeField] internal AttackTarget attackTarget;

    [SerializeField] internal List<Minion> targetMinions = new List<Minion>();
    [SerializeField] Minion targetMinion;
    [SerializeField] DefenseTower targetTower;

    public MinionStats minionStats;

    private void Start()
    {
        if (GetComponent<NetworkIdentity>().isClient)
        {
            Destroy(GetComponent<Rigidbody>());

            transform.GetChild(1).eulerAngles = new Vector3(0, owner * 180, 0);

            /*if (MatchManager.Instance.myPlayer.id == 0)
            {
                transform.GetChild(1).eulerAngles = new Vector3(0, owner * 180, 0);
            }
            else
            {
                transform.GetChild(1).eulerAngles = new Vector3(0, ((owner + 1) % 2) * 180, 0);
            }*/
        }

        minionStats.healthUI = transform.GetChild(0).GetChild(0).GetComponent<HealthUI>();
    }

    public void SetMinionData(MinionData _minionData)
    {
        minionStats = GetComponent<MinionStats>();

        attackInterval = _minionData.attackInterval;
        attackCountdown = _minionData.attackInterval;
        minionStats.minionType = _minionData.minionType;
        minionStats.health = _minionData.health;
        minionStats.damage = _minionData.damage;

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

            if (targetMinions.Count <= 0 || targetMinion == null)
            {
                if (targetTower)
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
    }

    public void AnotherTowerDestroyed(DefenseTower tower)
    {
        if (targetTower == tower)
        {
            targetTower = null;
            isAttacking = false;
            animationController.StartWalking();
            ChooseTargetMinion();
        }
    }

    internal virtual void AttackMinion(Minion attackTo) { }
    internal virtual void AttackTower(DefenseTower attackTo) { }

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
                    animationController.StartAttacking();

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
            isAttacking = true;
            animationController.StartAttacking();
            attackTarget = AttackTarget.Minion;
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
                attackCountdown = attackInterval;
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
