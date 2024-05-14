using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeypadController : MonoBehaviour
{
    // colocar referencia para porta
    public string password;
    public int passwordLimit;
    public Text passawordText;

    void Start()
    {
        passawordText.text = ""; 
    }

    public void PassawordEntry(string number)
    {
        if(number == "Clear")
        {
            Clear();
            return;
        }
        else if(number == "Enter")
        {
            Enter();
            return;         
        }

        int length = passawordText.text.ToString().Length;

        if(length < passwordLimit)
        {
            passawordText.text = passawordText.text + number;
        }
    }

    public void Clear()
    {
        passawordText.text = "";
        passawordText.color = Color.white;
    }

    private void Enter()
    {
        if(passawordText.text == password)
        {
            Debug.Log("Acertou");
            passawordText.color = Color.green;
            StartCoroutine(waitAndClear());
        }
        else
        {
            passawordText.color = Color.red;
            StartCoroutine(waitAndClear());
        }
    }

    IEnumerator waitAndClear()
    {
        yield return new WaitForSeconds(0.75f);
        Clear();
    }
}
