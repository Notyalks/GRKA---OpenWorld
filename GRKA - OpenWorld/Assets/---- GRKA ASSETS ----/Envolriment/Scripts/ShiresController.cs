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
    public GameObject portal2;
    public GameObject portal3;
    public GameObject portal4;
    public GameObject portalboss;
    public int fase;

    [Header("Shire Completion States")]
    public bool shire1Completed = false;
    public bool shire2Completed = false;
    public bool shire3Completed = false;
    public bool shire4Completed = false;

    private bool peca1Ativada = false;
    private bool peca2Ativada = false;
    private bool peca3Ativada = false;
    private bool peca4Ativada = false;

    private void Start()
    {
        fase = 0;

        // Desativar todos os portais no início
        portal2.SetActive(false);
        portal3.SetActive(false);
        portal4.SetActive(false);
        portalboss.SetActive(false);

        // Load initial states from PlayerPrefs
        shire1Completed = PlayerPrefs.GetInt("Shire1Finishe", 0) == 1;
        shire2Completed = PlayerPrefs.GetInt("Shire2Finishe", 0) == 1;
        shire3Completed = PlayerPrefs.GetInt("Shire3Finishe", 0) == 1;
        shire4Completed = PlayerPrefs.GetInt("Shire4Finishe", 0) == 1;

        UpdateShireStates();
    }

    void Update()
    {
        // Update PlayerPrefs based on the inspector values
        PlayerPrefs.SetInt("Shire1Finishe", shire1Completed ? 1 : 0);
        PlayerPrefs.SetInt("Shire2Finishe", shire2Completed ? 1 : 0);
        PlayerPrefs.SetInt("Shire3Finishe", shire3Completed ? 1 : 0);
        PlayerPrefs.SetInt("Shire4Finishe", shire4Completed ? 1 : 0);

        // Check and activate pieces if the Shires are completed
        if (shire1Completed && !peca1Ativada)
        {
            peca1.SetActive(true);
            peca1Ativada = true;
            fase++;
            portal2.SetActive(true);
        }

        if (shire2Completed && shire1Completed && !peca2Ativada)
        {
            peca2.SetActive(true);
            peca2Ativada = true;
            fase++;
            portal3.SetActive(true);
        }

        if (shire3Completed && shire2Completed && !peca3Ativada)
        {
            peca3.SetActive(true);
            peca3Ativada = true;
            fase++;
            portal4.SetActive(true);
        }

        if (shire4Completed && shire3Completed && !peca4Ativada)
        {
            peca4.SetActive(true);
            peca4Ativada = true;
            fase++;
            portalboss.SetActive(true);
        }
    }

    private void UpdateShireStates()
    {
        // Update the states of pieces and portals based on PlayerPrefs
        if (shire1Completed)
        {
            peca1.SetActive(true);
            peca1Ativada = true;
            fase++;
            portal2.SetActive(true);
        }

        if (shire2Completed && shire1Completed)
        {
            peca2.SetActive(true);
            peca2Ativada = true;
            fase++;
            portal3.SetActive(true);
        }

        if (shire3Completed && shire2Completed)
        {
            peca3.SetActive(true);
            peca3Ativada = true;
            fase++;
            portal4.SetActive(true);
        }

        if (shire4Completed && shire3Completed)
        {
            peca4.SetActive(true);
            peca4Ativada = true;
            fase++;
            portalboss.SetActive(true);
        }
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reset the local variables to reflect the cleared PlayerPrefs
        shire1Completed = false;
        shire2Completed = false;
        shire3Completed = false;
        shire4Completed = false;

        peca1.SetActive(false);
        peca2.SetActive(false);
        peca3.SetActive(false);
        peca4.SetActive(false);
        portal2.SetActive(false);
        portal3.SetActive(false);
        portal4.SetActive(false);
        portalboss.SetActive(false);

        peca1Ativada = false;
        peca2Ativada = false;
        peca3Ativada = false;
        peca4Ativada = false;

        fase = 0;
    }
}
