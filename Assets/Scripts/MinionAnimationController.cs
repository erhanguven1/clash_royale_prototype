using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionAnimationController : NetworkBehaviour
{
    public Animator animator;

    [Server]
    public void StartWalking()
    {
        animator.SetTrigger("Walk");
        // StartWalkingRPC();
    }

    [ClientRpc]
    private void StartWalkingRPC() //Doesn't work properly. Network Team Should Look At This.
    {
        animator.SetTrigger("Walk");
    }

    [Server]
    public void StartAttacking()
    {
        animator.SetTrigger("Attack");
        StartAttackingRPC();
    }

    [ClientRpc]
    private void StartAttackingRPC() //Animations doesn't work on client only on server.
    {
        animator.SetTrigger("Attack");
    }
}
