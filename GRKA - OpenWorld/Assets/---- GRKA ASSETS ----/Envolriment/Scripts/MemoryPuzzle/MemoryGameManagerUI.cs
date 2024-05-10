using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryGameManagerUI : MonoBehaviour
{
    public static MemoryGameManagerUI Instance { get; private set; }

    [SerializeField] private CardGroup cardGroup;
    [SerializeField] private List<CardSingleUI> CardSingleUIList = new List<CardSingleUI>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cardGroup.OnCardMatch += CardGroup_OnCardMatch;
    }

    private void CardGroup_OnCardMatch(object sender,System.EventArgs e)
    {
        //if (CardSingleUIList.All(CardSingleUI x => x.ObjectMatch() == true))
        //{
        //    StartCoroutine(OnCompleteGame());
        //}
        
    }

    private IEnumerator OnCompleteGame()
    {
        yield return new WaitForSeconds(0.75f);
        //fazer algo quando ganhar o jogo
    }
}
