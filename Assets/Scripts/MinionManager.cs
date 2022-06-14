using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionManager : InstancableNB<MinionManager>
{
    private List<Minion> minions = new List<Minion>();

    public void AddMinion(Minion minion)
    {
        minions.Add(minion);
    }

    public void OnMinionDead(Minion minion)
    {
        foreach (var item in minions)
        {
            if (item != minion)
            {
                item.AnotherMinionDead(minion);
            }
        }

        minions.Remove(minion);
    }
}
