using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MOVE : MonoBehaviour
{
    public float velocity = 6f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
       if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0,0,1) * Time.deltaTime * velocity);
            anim.SetBool("FD", true);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetBool("FD", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * velocity);
            anim.SetBool("BK", true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            anim.SetBool("BK", false);
        }
    }

}
