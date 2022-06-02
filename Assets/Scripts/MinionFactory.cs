using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinionType { Volva, Berserker }

public abstract class MinionData
{
    internal MinionType minionType;

    private uint health;
    public void SetHealth(uint _health) { health = _health; }
    public void DecreaseHealth(uint _health) { health -= _health; }

    private uint damage;
    public void SetDamage(uint _dmg) { damage = _dmg; }
    public void DecreaseDamage(uint _dmg) { damage -= _dmg; }

    private uint attackInterval;
    private uint countDown;
}

public class MinionFactory : MonoBehaviour
{
    public List<GameObject> minionGraphics = new List<GameObject>();
    private Dictionary<MinionType, MinionData> minionDictionary = new Dictionary<MinionType, MinionData>();

    public void Init(string _minionData)
    {
        MinionData minionData = JsonUtility.FromJson<MinionData>(_minionData);

        minionDictionary.Add(minionData.minionType, minionData);
    }

    public void SpawnMinion(MinionType minionType)
    {
        var graphic = minionGraphics[(int)minionType];

        //Spawn minion parent
        var minion = new GameObject(minionType.ToString());

        //Spawn minion's graphic inside of the minion (parent)
        Instantiate(graphic, minion.transform);

        var minionComponent = minion.AddComponent<Minion>();

        minionComponent.minionData = minionDictionary[minionType];
    }
}