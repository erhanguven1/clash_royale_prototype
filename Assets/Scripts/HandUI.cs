using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CardListHand : List<GameObject>
{
    public new void Add(GameObject item)
    {
        base.Add(item);
        MatchManager.Instance.myPlayer.UpdateHandCardCount(1);
    }

    public new void Remove(GameObject item)
    {
        base.Remove(item);
        MatchManager.Instance.myPlayer.UpdateHandCardCount(-1);
    }
}

public class HandUI : Instancable<HandUI>
{
    public CardListHand cardsInHand = new CardListHand();

    public GameObject cardUIPrefab;
    public Transform handTransform;

    public void AddCard(Card card)
    {
        var cardUI = Instantiate(cardUIPrefab, handTransform).GetComponent<CardUI>();
        cardUI.SetCard(card);
        cardsInHand.Add(cardUI.gameObject);
    }

    public void RemoveCard(Card card)
    {
        var cardUI = cardsInHand.FirstOrDefault(c => c.GetComponent<CardUI>().card == card);
        cardsInHand.Remove(cardUI.gameObject);
        Destroy(cardUI);
    }
}
