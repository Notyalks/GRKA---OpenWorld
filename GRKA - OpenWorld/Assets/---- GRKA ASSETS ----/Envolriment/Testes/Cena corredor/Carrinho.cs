using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrinho : MonoBehaviour
{
    public Transform pontoInicial;
    public Transform pontoFinal;
    public float velocidade = 5.0f;
    public float damageAmount = 10f;

    private Vector3 destino;

    void Start()
    {
        destino = pontoFinal.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        if (transform.position == pontoFinal.position)
        {
            destino = pontoInicial.position;
        }
        else if (transform.position == pontoInicial.position)
        {
            destino = pontoFinal.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TomarDano(damageAmount);
            }
        }
    }
}
