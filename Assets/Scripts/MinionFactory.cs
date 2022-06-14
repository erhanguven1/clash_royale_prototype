using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum MinionType { Volva, Berserker }

[System.Serializable]
public class MinionData
{
    [SerializeField] internal MinionType minionType;

    [SerializeField] internal int health;

    [SerializeField] internal int damage;

    [SerializeField] internal int attackInterval;
}

public class MinionFactory : Instancable<MinionFactory>
{
    public GameObject minionPrefab;

    public List<GameObject> minionGraphics = new List<GameObject>();
    private Dictionary<MinionType, MinionData> minionDictionary = new Dictionary<MinionType, MinionData>();

    private void Start()
    {
        //Create test minions
        var m = new MinionData { minionType = MinionType.Volva, damage = 40, health = 100, attackInterval = 2 };
        Init(m);
    }

    public void Init(string _minionData)
    {
        MinionData minionData = JsonUtility.FromJson<MinionData>(_minionData);

        minionDictionary.TryAdd(minionData.minionType, minionData);
    }

    public void Init(MinionData _minionData)
    {
        MinionData minionData = _minionData;

        minionDictionary.TryAdd(minionData.minionType, minionData);
    }

    public NetworkIdentity SpawnMinion(MinionType minionType, Vector3 spawnPosition, int owner)
    {
        var graphic = minionGraphics[(int)minionType];

        //Spawn minion parent
        var minion = Instantiate(NetworkManager.singleton.spawnPrefabs[1]);

        minion.name = minionType.ToString();

        minion.transform.position = spawnPosition;

        Minion minionComponent = null;

        switch (minionType)
        {
            case MinionType.Volva:
                minionComponent = minion.AddComponent<Volva>();
                break;
            case MinionType.Berserker:
                minionComponent = minion.AddComponent<Volva>();
                break;
            default:
                break;
        }

        //Spawn minion's graphic inside of the minion (parent)
        var minionGraphic = Instantiate(graphic, minion.transform).transform.GetChild(0);
        minionComponent.animationController = minionGraphic.GetComponent<MinionAnimationController>();
        minionGraphic.transform.eulerAngles = new Vector3(0, owner * 180, 0);

        minionComponent.SetMinionData(minionDictionary[minionType]);
        minionComponent.owner = owner;

        MinionManager.Instance.AddMinion(minionComponent);

        NetworkServer.Spawn(minion, CR_NetworkManager.instance.conns[owner]);

        return minion.GetComponent<NetworkIdentity>();

    }

    public void InitMinionOnClient(NetworkIdentity networkIdentity, MinionType minionType, Vector3 spawnPosition, int owner)
    {
        var graphic = minionGraphics[(int)minionType];

        //Spawn minion parent
        var minion = networkIdentity.gameObject;

        //Spawn minion's graphic inside of the minion (parent)
        Instantiate(graphic, minion.transform);

        Minion minionComponent = null;

        switch (minionType)
        {
            case MinionType.Volva:
                minionComponent = minion.AddComponent<Volva>();
                break;
            case MinionType.Berserker:
                minionComponent = minion.AddComponent<Volva>();
                break;
            default:
                break;
        }

        minionComponent.SetMinionData(minionDictionary[minionType]);
        minionComponent.owner = owner;
    }
}