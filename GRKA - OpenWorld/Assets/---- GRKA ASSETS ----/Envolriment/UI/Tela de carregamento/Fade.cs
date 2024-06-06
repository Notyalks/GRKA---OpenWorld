using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public Animator anim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Load());
        }

    }

    private IEnumerator Load()
    {
        yield return null;

        anim.SetTrigger("fade");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(0);
    }
}
