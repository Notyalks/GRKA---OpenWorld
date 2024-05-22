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

    

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("Shire1Finishe") == 1)
        {
            peca1.SetActive(true);
            fase++;
        }
        if (PlayerPrefs.GetInt("Shire2Finishe") == 1)
        {
            peca2.SetActive(true);
            fase++;
        }
        if (PlayerPrefs.GetInt("Shire3Finishe") == 1)
        {
            peca3.SetActive(true);
            fase++;
        }
        if (PlayerPrefs.GetInt("Shire4Finishe") == 1)
        {
            peca4.SetActive(true);
            fase++;
        }
        if(fase == 4)
        {
            portal.SetActive(true);
        }
    }
}
