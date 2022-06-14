using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : Instancable<ArenaManager>
{
    public Vector3 arenaForward;

    //0 = top left, 1 = top right, 2 = down left, 3 = down right
    public List<Transform> towerTransforms;

    public void SetArenaRotation(int playerId)
    {
        transform.eulerAngles = Vector3.up * (playerId == 0 ? 0 : 180);

        arenaForward = playerId == 0 ? Vector3.forward : Vector3.back;

        print("id = " + playerId);
    }

}

public static class ArenaExtentions
{
    public static Vector3 ArenaDirection(this Minion minion)
    {
        return (minion.owner * 2 - 1) * Vector3.back;
    }
}