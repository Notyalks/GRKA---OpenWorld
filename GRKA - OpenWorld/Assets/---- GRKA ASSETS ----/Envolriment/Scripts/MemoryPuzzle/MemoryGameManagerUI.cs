using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryGameManagerUI : MonoBehaviour
{
    public static MemoryGameManagerUI Instance { get; private set; }

    [SerializeField] private CardGroup cardGroup;
    [SerializeField] private List<CardSingleUI> CardSingleUIList = new List<CardSingleUI>();

    [SerializeField] private GameObject gameArea;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);

        DifficultyManager.Instance
            .ResetListeners()
            .OnEasyButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);
            })
            .OnNormalButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);
            })
            .OnHardButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);
            });
    }

    private void Start()
    {
        cardGroup.OnCardMatch += CardGroup_OnCardMatch;
    }

    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (CardSingleUIList == null)
        {
            CardSingleUIList = new List<CardSingleUI>();
        }

        if (!CardSingleUIList.Contains(cardSingleUI))
        {
            CardSingleUIList.Add(cardSingleUI);
        }
    }

    private void CardGroup_OnCardMatch(object sender,System.EventArgs e)
    {
        if(CardSingleUIList.All(X => X.GetObjectMatch() == true))
        {
           StartCoroutine(OnCompleteGame());
        }
        
    }

    private IEnumerator OnCompleteGame()
    {
        yield return new WaitForSeconds(0.75f);
        //fazer algo quando ganhar o jogo
        Debug.Log("ganhou");
    }

    public DifficultyEnum GetDifficulty()
    {
        return DifficultyManager.Instance.GetDifficulty();
    }

    public void Restart()
    {
        CardSingleUIList.Clear();
    }
    private void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    private void ToggleGameArea(bool toggle)
    {
        gameArea.SetActive(toggle);
    }
}
