using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CR_NetworkManager : NetworkManager
{
    public MatchManager mm;

    public static CR_NetworkManager instance;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }

    public List<NetworkConnectionToClient> conns = new List<NetworkConnectionToClient>();

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        conns.Add(conn);

        if (NetworkServer.connections.Count == 2)
        {
            mm = Instantiate(spawnPrefabs[0]).GetComponent<MatchManager>();
            NetworkServer.Spawn(mm.gameObject);

            var player1 = Instantiate(playerPrefab);
            NetworkServer.Spawn(player1, conns[0]);
            NetworkServer.AddPlayerForConnection(conns[0], player1);
            player1.GetComponent<Player>().SetID(0);

            var player2 = Instantiate(playerPrefab);
            NetworkServer.Spawn(player2, conns[1]);
            NetworkServer.AddPlayerForConnection(conns[1], player2);
            player2.GetComponent<Player>().SetID(1);

            StartCoroutine(wait());

            IEnumerator wait()
            {
                yield return new WaitForSeconds(1);
                player1.GetComponent<Player>().SetPlayer();
                player2.GetComponent<Player>().SetPlayer();

                var player1LeftTower = Instantiate(spawnPrefabs[2]);
                NetworkServer.Spawn(player1LeftTower, conns[0]);
                player1LeftTower.GetComponent<DefenseTower>().owner = (0);
                player1LeftTower.GetComponent<DefenseTower>().SetTowerPosition(2);

                var player1RightTower = Instantiate(spawnPrefabs[2]);
                NetworkServer.Spawn(player1RightTower, conns[0]);
                player1RightTower.GetComponent<DefenseTower>().owner = (0);
                player1RightTower.GetComponent<DefenseTower>().SetTowerPosition(3);

                var player2LeftTower = Instantiate(spawnPrefabs[2]);
                NetworkServer.Spawn(player2LeftTower, conns[1]);
                player2LeftTower.GetComponent<DefenseTower>().owner = (1);
                player2LeftTower.GetComponent<DefenseTower>().SetTowerPosition(0);

                var player2RightTower = Instantiate(spawnPrefabs[2]);
                NetworkServer.Spawn(player2RightTower, conns[1]);
                player2RightTower.GetComponent<DefenseTower>().owner = (1);
                player2RightTower.GetComponent<DefenseTower>().SetTowerPosition(1);
            }
        }
    }

    void SpawnTowers()
    {

    }
}
