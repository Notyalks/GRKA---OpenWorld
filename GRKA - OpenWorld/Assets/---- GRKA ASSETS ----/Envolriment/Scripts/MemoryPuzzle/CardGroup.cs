using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>();
    [SerializeField] private List<CardSingleUI> selectedCardList = new List<CardSingleUI>();

    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (cardSingleUIList == null)
        {
            cardSingleUIList = new List<CardSingleUI>();
        }
        if (!cardSingleUIList.Contains(cardSingleUI))
        {
            cardSingleUIList.Add(cardSingleUI);
        }
    }

    public void OnCardSelected(CardSingleUI cardSingleUI)
    {
        selectedCardList.Add(cardSingleUI);
        cardSingleUI.Select();
    }
}
