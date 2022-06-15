using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private float attackCountdown = 2;
    [SerializeField] internal int attackInterval;
    [SerializeField] internal bool isAttacking;
    [SerializeField] internal AttackTarget attackTarget;
    // internal virtual void AttackMinion(Minion attackTo) { }
    internal List<Minion> targetMinions = new List<Minion>();
    [SerializeField] Minion targetMinion;
    private int damage = 70;
    
    
    void AttackMinion(Minion attackTo)
    {
        print("attacked");
        attackTo.TakeDamage(damage);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        var otherMinion = other.GetComponent<Minion>();

        if (otherMinion != null)
        {
            print("Tower encountered with minion!");

            // if (otherMinion.IsEnemyWith(this))
            {
                targetMinions.Add(otherMinion);
                ChooseTargetMinion();

                if (!isAttacking)
                {
                    isAttacking = true;
                    // animationController.StartAttacking();

                    attackTarget = AttackTarget.Minion;
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
            // animationController.StartWalking();

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

    private void Update()
    {
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
            }
        }
    }
}
