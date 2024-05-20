using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shires : MonoBehaviour
{
    bool podeInteragir = false;
    public GameObject Jogador;
    public string nextLevel;
    
    void Update()
    {
        if (podeInteragir == true && Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = 0f;
            Jogador.GetComponent<SalvarPosic>().SalvarLocalizacao();
            SceneManager.LoadScene(nextLevel);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
        }
       
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
        }
        
    }


    void OnGUI()
    {
        if(podeInteragir == true)
        {
            GUIStyle stylez = new GUIStyle();
            stylez.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 200, 30), "Pressione E");
        }
        
    } 

    


}
