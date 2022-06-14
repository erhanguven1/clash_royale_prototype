using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : Instancable<CardUI>, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Card card;
    public GameObject cardElements;
    Canvas canvas;

    Vector2 lastPos;

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    public void SetCard(Card _card)
    {
        card = _card;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = (transform as RectTransform).anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        (transform as RectTransform).anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (eventData.position.y > Screen.height * .25f)
        {
            cardElements.SetActive(false);
            MinionSpawner.Instance.SelectMinion(card.minionType, this);
        }
        else if(!cardElements.activeInHierarchy)
        {
            cardElements.SetActive(true);
            MinionSpawner.Instance.DeselectMinion();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.position.y <= Screen.height * .25f)
        {
            (transform as RectTransform).anchoredPosition = lastPos;
            MinionSpawner.Instance.DeselectMinion();
        }
        else
        {
            (transform as RectTransform).anchoredPosition = lastPos;
            cardElements.SetActive(true);
        }
    }

    public void OnMinionSpawned()
    {
        DrawManager.Instance.AddCardToDrawPile(card);
        HandUI.Instance.RemoveCard(card);
        Destroy(this.gameObject);
    }
}
