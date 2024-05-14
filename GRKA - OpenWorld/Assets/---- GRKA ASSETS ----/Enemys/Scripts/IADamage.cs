using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IADamage : MonoBehaviour
{
    public int live = 10;
    public Cactus cactus;
    
    void Update()
    {
        if (live < 0)
        {
            cactus.Dead();
            Destroy(gameObject, 4);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            live--;
            cactus.Damage();
        }
    }

    public void ExplosionDamage()
    {
        live = -1;
    }
}
