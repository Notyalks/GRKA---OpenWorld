using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public Animator anim;

    public void StartFade(int sceneIndex)
    {
        StartCoroutine(FadeRoutine(sceneIndex));
    }

    private IEnumerator FadeRoutine(int sceneIndex)
    {
        anim.SetTrigger("fade");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneIndex);
    }
}
