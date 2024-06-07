using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    [Header("Porcentagem")]
    public Slider sLoading;
    public Text txtPorcentagem;

    [Header("Imagem")]
    public Image imgParaMudar;
    public Sprite[] imagens;

    private AsyncOperation operation;

    private void Start()
    {
        sLoading.value = 0;
        txtPorcentagem.text = "0%";
    }

    public void MudarImagem()
    {
        int rand = Random.Range(0, imagens.Length);
        imgParaMudar.sprite = imagens[rand];
    }

    public void StartLoading(AsyncOperation operation, System.Action onComplete)
    {
        this.operation = operation;
        StartCoroutine(LoadSceneStylish(onComplete));
    }

    private IEnumerator LoadSceneStylish(System.Action onComplete)
    {
        operation.allowSceneActivation = false;

        float progresso = 0.0f;

        while (progresso < 100)
        {
            yield return new WaitForSeconds(0.8f);

            progresso += Random.Range(5.0f, 15.0f);
            sLoading.DOValue(progresso, 0.5f);
            txtPorcentagem.text = ((int)progresso) + "%";
        }

        sLoading.DOValue(100, 0.5f);
        txtPorcentagem.text = "100%";

        yield return new WaitForSeconds(0.5f);

        onComplete?.Invoke();

        operation.allowSceneActivation = true;
    }
}
