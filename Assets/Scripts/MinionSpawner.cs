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
    [SerializeField] private CardUI selectedCard;

    public GameObject spawnPointVisual;

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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                spawnPointVisual.transform.position = hit.point;

                if (Input.GetMouseButtonUp(0))
                {
                    //Tell the server to spawn a minion for me
                    MatchManager.Instance.RequestSpawnMinion(MatchManager.Instance.myPlayer.id, selectedMinionType, GetPointOnLane(hit.point.z, hit.collider.transform.position));
                    selectedCard.OnMinionSpawned();
                    DeselectMinion();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DeselectMinion();
            }
        }
    }

    public void SelectMinion(MinionType minionType, CardUI _selectedCard)
    {
        if (isMinionSelected)
        {
            return;
        }
        selectedCard = _selectedCard;
        selectedMinionType = minionType;
        isMinionSelected = true;
    }

    public void DeselectMinion()
    {
        if (!isMinionSelected)
        {
            return;
        }

        selectedCard = null;
        spawnPointVisual.transform.position = Vector3.down * 1000;
        isMinionSelected = false;
    }

    internal NetworkIdentity SpawnMinion(int _owner, MinionType _minionType, Vector3 _spawnPosition)
    {
        return MinionFactory.Instance.SpawnMinion(selectedMinionType, _spawnPosition, _owner);
    }

    private Vector3 GetPointOnLane(float hitPointZ, Vector3 lanePosition)
    {
        return new Vector3(lanePosition.x, lanePosition.y + 0, hitPointZ);
    }
}
