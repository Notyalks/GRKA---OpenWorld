using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuMainManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject painelMainMenu;
    [SerializeField] private GameObject painelControles;
    [SerializeField] private GameObject painelCreditos;

   public void Comecar()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        SceneController.instance.LoadScene(nomeDoLevelDeJogo); // Substitua "GameScene" pelo nome da cena que você deseja carregar
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
