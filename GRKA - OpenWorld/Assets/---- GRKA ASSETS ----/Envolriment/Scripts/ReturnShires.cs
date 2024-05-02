using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnShires : MonoBehaviour
{
    public string level;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MyLoadScene());
        }
    }

    IEnumerator MyLoadScene()
    {

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(level);
        
    }
}
