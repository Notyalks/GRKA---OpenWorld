using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SalvarPosic : MonoBehaviour
{
    string cenaAtual;
    public float subiu = 0f;
    
    void Awake()
    {
        cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log(cenaAtual);
    }

    
    public void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey(cenaAtual + "X") && PlayerPrefs.HasKey(cenaAtual + "Y") && PlayerPrefs.HasKey(cenaAtual + "Z"))
            {
                transform.position = new Vector3(PlayerPrefs.GetFloat(cenaAtual + "X"), PlayerPrefs.GetFloat(cenaAtual + "Y"), PlayerPrefs.GetFloat(cenaAtual + "Z"));
                Debug.Log("mandouumnovo");
            }

    }

    public void SalvarLocalizacao()
    {
        PlayerPrefs.SetFloat(cenaAtual + "X", transform.position.x);
        PlayerPrefs.SetFloat(cenaAtual + "Y", transform.position.y);
        PlayerPrefs.SetFloat(cenaAtual + "Z", transform.position.z);
    } 

  

}

