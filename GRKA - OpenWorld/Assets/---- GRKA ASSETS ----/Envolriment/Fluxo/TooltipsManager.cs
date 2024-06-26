using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    public GameObject tooltipPanel;
    public Text tooltipText; // Use TextMeshProUGUI for TextMeshPro or Text for regular Text
    public float displayDuration = 2f; // Duration for how long the tooltip will be visible

    private Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        animator = tooltipPanel.GetComponent<Animator>();
    }

    void Start()
    {
    
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        animator.SetTrigger("Show");
        StartCoroutine(HideTooltipAfterDelay(displayDuration));
    }

    private IEnumerator HideTooltipAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Hide");
    }
}

