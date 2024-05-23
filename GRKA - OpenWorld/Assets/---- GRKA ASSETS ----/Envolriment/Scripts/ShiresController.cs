using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiresController : MonoBehaviour
{

    public GameObject peca1;
    public GameObject peca2;
    public GameObject peca3;
    public GameObject peca4;
    public GameObject portal;
    public int fase;
    private bool peca1Ativada = false;
    private bool peca2Ativada = false;
    private bool peca3Ativada = false;
    private bool peca4Ativada = false;

    private void Start()
    {
        fase = 0;
    }


    // Update is called once per frame
    void Update()
    {

        if(PlayerPrefs.GetInt("Shire1Finishe") == 1 && !peca1Ativada)
        {
            peca1.SetActive(true);
            peca1Ativada = true;
            fase++;
        }

        if (PlayerPrefs.GetInt("Shire2Finishe") == 1 && !peca2Ativada)
        {
            peca2.SetActive(true);
            peca2Ativada = true;
            fase++;
        }

        if (PlayerPrefs.GetInt("Shire3Finishe") == 1 && !peca3Ativada)
        {
            peca3.SetActive(true);
            peca3Ativada = true;
            fase++;
        }

        if (PlayerPrefs.GetInt("Shire4Finishe") == 1 && !peca4Ativada)
        {
            peca4.SetActive(true);
            peca4Ativada = true;
            fase++;
        }

        if(fase == 4)
        {
            portal.SetActive(true);
        }
    }
}
