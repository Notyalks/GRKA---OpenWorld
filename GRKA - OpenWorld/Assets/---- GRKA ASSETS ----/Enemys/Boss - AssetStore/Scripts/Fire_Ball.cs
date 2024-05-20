using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Ball : MonoBehaviour
{
    private float cronometro;

    

    void Update()
    {
      transform.Translate(Vector3.forward * 6 * Time.deltaTime);
      transform.localScale += new Vector3(3, 3, 3) * Time.deltaTime;

      cronometro += 1 * Time.deltaTime;

      if(cronometro > 1)
      {
            transform.localScale = new Vector3(1, 1, 1);
            gameObject.SetActive(false);
            cronometro = 0;
      }
    }
}
