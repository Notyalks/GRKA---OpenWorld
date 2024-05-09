using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectablesHabb : MonoBehaviour
{
    public int shire1Finishe;
    public int shire2Finishe;
    public int shire3Finishe;
    public int shire4Finishe;

    private void OnTriggerEnter(Collider other)
    {
        if (SceneManager.GetActiveScene().name == "Shire1")
        {
            if (other.CompareTag("Player"))
            {
                shire1Finishe = 1;
                PlayerPrefs.SetInt("Shire1Finishe", shire1Finishe);
                PlayerPrefs.Save();
                Destroy(this.gameObject);
            }
        }
        else if(SceneManager.GetActiveScene().name == "Shire2")
        {
            if (other.CompareTag("Player"))
            {
                shire2Finishe = 1;
                PlayerPrefs.SetInt("Shire2Finishe", shire2Finishe);
                PlayerPrefs.Save();
                Destroy(this.gameObject);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Shire3")
        {
            if (other.CompareTag("Player"))
            {
                shire3Finishe = 1;
                PlayerPrefs.SetInt("Shire3Finishe", shire3Finishe);
                PlayerPrefs.Save();
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                shire4Finishe = 1;
                PlayerPrefs.SetInt("Shire4Finishe", shire4Finishe);
                PlayerPrefs.Save();
                Destroy(this.gameObject);
            }
        }
    }
}
