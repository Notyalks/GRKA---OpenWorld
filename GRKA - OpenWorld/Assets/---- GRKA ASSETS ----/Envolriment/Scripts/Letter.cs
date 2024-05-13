using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public GameObject letterUI;
    bool toggle;
    public PlayerManager playerManager;
    public Renderer letterMesh;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public void openCloseLetter()
    {
        toggle = !toggle;   
        if(toggle == false)
        {
            letterUI.SetActive(false);
            playerManager.canMoveCam = true;
            letterMesh.enabled = true;
            Time.timeScale = 1f;
        }
        if(toggle == true)
        {
            letterUI.SetActive(true);
            playerManager.canMoveCam = false;
            letterMesh.enabled = false;
            Time.timeScale = 0f;
        }
    }
}
