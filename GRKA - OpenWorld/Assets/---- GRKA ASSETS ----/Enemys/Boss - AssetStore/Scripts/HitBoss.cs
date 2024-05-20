using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoss : MonoBehaviour
{
    PlayerManager playerManager;
    public int damage;
    public float vidareal = 100f;
    public bool atacou;

    private void Start()
    {
        atacou = false;
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            atacou = true;
        }
    }

    
}
