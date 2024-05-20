using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangoBoss : MonoBehaviour
{
    public Animator ani;
    public Boss boss;
    public int melee;


    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            melee = Random.Range(0, 4);
            switch (melee)
            {
                case 0:
                    ani.SetFloat("skills", 0);
                    boss.hit_select = 0;
                    break;
                case 1:
                    ani.SetFloat("skills", 0.2f);
                    boss.hit_select = 1;
                    break;
                case 2:
                    ani.SetFloat("skills", 0.4f);
                    boss.hit_select = 2;
                    break;
                case 3:
                   if(boss.fase == 2)
                    {
                        ani.SetFloat("skills", 1);
                    }
                    else
                    {
                        melee = 0;
                    }
                   break;
            }
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", true);
            boss.atacando = true;
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
