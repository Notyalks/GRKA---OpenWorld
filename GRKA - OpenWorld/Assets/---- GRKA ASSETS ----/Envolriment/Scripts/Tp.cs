using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tp : MonoBehaviour
{
    [SerializeField] Transform tp;
    [SerializeField] GameObject Player;
    private bool playerInsideTp = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInsideTp)
        {
            Debug.Log("Apertou");
            StartCoroutine(Teleport());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTp = false;
        }
    }


    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(1);
        Player.transform.position = new Vector3(tp.transform.position.x, tp.transform.position.y, tp.transform.position.z);
    }
}
