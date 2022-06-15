using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] internal int id;
    [SerializeField] [SyncVar(hook = nameof(OnReceiveMana))] private int mana;

    private void OnReceiveMana(int _old, int _new)
    {
        if (hasAuthority)
        {
            ManaBar.Instance.UpdateValue(_new);

        }
    }

    public int cardsInHandCount;
    public int cardsInDrawPileCount;

    public int GetMana()
    {
        return mana;
    }

    [Command(requiresAuthority = false)]
    public void UseManaOnServer(int cardMana)
    {
        mana -= cardMana;
        mana = Mathf.Clamp(mana, 0, 10);
    }

    [Server]
    public void SetID(int _id)
    {
        id = _id;
        mana = 0;
        SetIDOnClients(id);
        StartCoroutine(Timer());
    }

    [ClientRpc]
    void SetIDOnClients(int _id)
    {
        id = _id;
    }

    [ClientRpc]
    public void SetPlayer()
    {
        if (hasAuthority)
        {
            MatchManager.Instance.myPlayer = this;
            ArenaManager.Instance.SetArenaRotation(id);

            DrawManager.Instance.CreateTestCards();

            print("has authority!");
        }
        else
        {
            MatchManager.Instance.opponentPlayer = this;
            print("no authority!");
        }
    }

    //Add 1 mana every second (SERVER)
    public IEnumerator Timer()
    {
        while (true)
        {
            mana += 1;
            mana = Mathf.Clamp(mana, 0, 10);
            yield return new WaitForSeconds(1);
            mana += 1;
            mana = Mathf.Clamp(mana, 0, 10);
            yield return new WaitForSeconds(1);
            TryDrawCard();
        }
    }

    [Server]
    private void TryDrawCard()
    {
        if (cardsInDrawPileCount > 0 && cardsInHandCount < 5)
        {
            RPCDrawCard();
        }
    }

    [ClientRpc]
    private void RPCDrawCard()
    {
        if (hasAuthority)
        {
            DrawManager.Instance.DrawCard();
        }
    }

    [Command(requiresAuthority = false)]
    public void UpdateDeckCardCount(int delta)
    {
        cardsInDrawPileCount += delta;
    }

    [Command(requiresAuthority = false)]
    public void UpdateHandCardCount(int delta)
    {
        cardsInHandCount += delta;
    }
}
