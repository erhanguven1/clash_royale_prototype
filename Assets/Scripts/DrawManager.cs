using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardListDrawPile : List<Card>
{
    public new void Add(Card item)
    {
        base.Add(item);
        MatchManager.Instance.myPlayer.UpdateDeckCardCount(1);
    }

    public new void Remove(Card item)
    {
        base.Remove(item);
        MatchManager.Instance.myPlayer.UpdateDeckCardCount(-1);
    }
}

public class DrawManager : Instancable<DrawManager>
{
    public CardListDrawPile cardsInDrawPile = new CardListDrawPile();

    public void CreateTestCards()
    {
        print("creating test cards...");
        for (int i = 0; i < 15; i++)
        {
            var card = new Card(MinionType.Volva, 3);
            cardsInDrawPile.Add(card);
        }
    }

    public void DrawCard()
    {
        var cardToDraw = cardsInDrawPile.First();
        HandUI.Instance.AddCard(cardToDraw);
        cardsInDrawPile.Remove(cardToDraw);

    }

    public void AddCardToDrawPile(Card card)
    {
        cardsInDrawPile.Add(card);
    }
}
