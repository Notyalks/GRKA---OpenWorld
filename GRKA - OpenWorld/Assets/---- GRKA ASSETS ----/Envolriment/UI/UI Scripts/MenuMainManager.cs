using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMainManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMainMenu;
    [SerializeField] private GameObject painelControles;
    [SerializeField] private GameObject painelCreditos;

   public void Comecar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void AbrirControles()
    {
        painelMainMenu.SetActive(false);
        painelControles.SetActive(true);
    }

    public void FecharControles()
    {
        painelControles.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void AbrirCreditos()
    {
        painelMainMenu.SetActive(false);
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
    }
}
