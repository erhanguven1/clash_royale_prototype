using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : InstancableNB<TowerManager>
{
    private List<DefenseTower> towers = new List<DefenseTower>();

    public void AddMinion(DefenseTower tower)
    {
        towers.Add(tower);
    }

    public void OnTowerDestroyed(DefenseTower tower)
    {
        var minions = MinionManager.Instance.GetMinions();

        foreach (var item in minions)
        {
            item.AnotherTowerDestroyed(tower);
        }

        towers.Remove(tower);
    }
}
