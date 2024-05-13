using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCams : MonoBehaviour
{
    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject Cam3;
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject canvas3;


    public GameObject canvasReal;
    public GameObject camReal;

    public bool apertou;

    public void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entrou");

        if(this.gameObject.name == "Sphere1")
        {
            
            canvasReal.SetActive(false);
            camReal.SetActive(false);
            canvas1.SetActive(true);
            Cam1.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            
        }

        if (this.gameObject.name == "Sphere2")
        {

            canvasReal.SetActive(false);
            camReal.SetActive(false);
            canvas2.SetActive(true);
            Cam2.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            
        }

        if (this.gameObject.name == "Sphere3")
        {
            canvasReal.SetActive(false);
            camReal.SetActive(false);
            canvas3.SetActive(true);
            Cam3.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.gameObject.name == "Sphere1")
        {
            Cam1.SetActive(false);
            canvas1.SetActive(false);
            Destroy(this.gameObject);
        }
        if (this.gameObject.name == "Sphere2")
        {
            Cam2.SetActive(false);
            canvas2.SetActive(false);
            Destroy(this.gameObject);
        }
        if (this.gameObject.name == "Sphere3")
        {
            Cam3.SetActive(false);
            canvas3.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}
