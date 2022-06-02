using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour
{
    internal MinionData minionData;

    public void TakeDamage(uint dmg)
    {
        minionData.DecreaseHealth(dmg);
    }
}
