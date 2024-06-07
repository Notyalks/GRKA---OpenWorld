using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public Fade fade; // Referência para o componente Fade

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Chama o fade in
        if (fade.anim != null)
            fade.anim.SetTrigger("fade");

        // Aguarda a animação de fade
        yield return new WaitForSeconds(1.0f);

        // Carrega a tela de carregamento
        AsyncOperation loadingScreenOperation = SceneManager.LoadSceneAsync("LoadingScreen");
        while (!loadingScreenOperation.isDone)
        {
            yield return null;
        }

        // Aguarda um frame para garantir que a tela de carregamento esteja ativa
        yield return null;

        // Carrega a nova cena
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
        sceneLoadingOperation.allowSceneActivation = false;

        // Acessa o SceneLoader na tela de carregamento e inicia o processo estiloso de carregamento
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.StartLoading(sceneLoadingOperation, () =>
            {
                // Chama o fade out
                if (fade.anim != null)
                    fade.anim.SetTrigger("fade");

                // Aguarda a animação de fade
                StartCoroutine(WaitAndAllowSceneActivation(sceneLoadingOperation));
            });

            while (!sceneLoadingOperation.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("SceneLoader not found in the scene.");
        }
    }

    private IEnumerator WaitAndAllowSceneActivation(AsyncOperation sceneLoadingOperation)
    {
        yield return new WaitForSeconds(1.5f);
        sceneLoadingOperation.allowSceneActivation = true;
    }
}
