using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class TowerBehaviour : NetworkBehaviour
{
    private float attackCountdown = 2;
    [SerializeField] internal int attackInterval;
    [SerializeField] internal bool isAttacking;
    [SerializeField] internal List<Minion> targetMinions = new List<Minion>();
    [SerializeField] Minion targetMinion;
    private int damage = 70;

    [SerializeField] private DefenseTower defenseTower;

    void AttackMinion(Minion attackTo)
    {
        print("attacked");
        attackTo.TakeDamage(damage);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isClient)
        {
            return;
        }

        var otherMinion = other.GetComponent<Minion>();

        if (otherMinion != null)
        {
            print("Tower encountered with minion!");

            if (IsEnemyWith(otherMinion))
            {
                targetMinions.Add(otherMinion);
                ChooseTargetMinion();

                if (!isAttacking)
                {
                    isAttacking = true;
                }
            }

            return;
        }
    }

    
    void ChooseTargetMinion()
    {
        targetMinion = null;

        if (targetMinions.Count == 0)
        {
            isAttacking = false;
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
    public void AnotherMinionDead(Minion minion)
    {
        if (targetMinions.Contains(minion))
        {
            targetMinions.Remove(minion);

            if (targetMinion == minion)
            {
                isAttacking = false;
                ChooseTargetMinion();
            }
        }
    }

    public bool IsEnemyWith(Minion minion)
    {
        if (minion == null || defenseTower == null)
        {
            return false;
        }

        return minion.owner != defenseTower.owner;
    }
    private void Update()
    {
        if (isClient)
        {
            return;
        }

        if (isAttacking)
        {
            attackCountdown -= Time.deltaTime;

            if (attackCountdown <= 0)
            {
                attackCountdown = attackInterval;

                if (targetMinion.minionStats.health < damage)
                {
                    AttackMinion(targetMinion);
                    targetMinions.Remove(targetMinion);
                    isAttacking = false;
                    ChooseTargetMinion();
                }
                else
                {
                    AttackMinion(targetMinion);
                }
            }
        }
    }
}
