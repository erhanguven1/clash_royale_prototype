using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : Instancable<MinionSpawner>
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool isMinionSelected;
    [SerializeField] private MinionType selectedMinionType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If there is a minion selected
        if (isMinionSelected)
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                //Tell the server to spawn a minion for me
                MatchManager.Instance.RequestSpawnMinion(MatchManager.Instance.myPlayer.id, selectedMinionType, GetPointOnLane(hit.point.z, hit.collider.transform.position));
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isMinionSelected = true;
            selectedMinionType = MinionType.Volva;
        }
    }

    internal NetworkIdentity SpawnMinion(int _owner, MinionType _minionType, Vector3 _spawnPosition)
    {
        return MinionFactory.Instance.SpawnMinion(selectedMinionType, _spawnPosition, _owner);
    }

    private Vector3 GetPointOnLane(float hitPointZ, Vector3 lanePosition)
    {
        return new Vector3(lanePosition.x, lanePosition.y + 1, hitPointZ);
    }
}
