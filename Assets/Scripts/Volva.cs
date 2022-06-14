using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volva : Minion
{
    internal override void AttackMinion(Minion attackTo)
    {
        attackTo.TakeDamage(minionStats.damage);
    }

    internal override void AttackTower(DefenseTower attackTo)
    {
        attackTo.TakeDamage(minionStats.damage);
    }
}
