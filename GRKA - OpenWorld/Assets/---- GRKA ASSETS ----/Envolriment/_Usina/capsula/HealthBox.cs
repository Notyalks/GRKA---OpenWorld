using UnityEngine;
using System.Collections;

public class HealthBox : MonoBehaviour
{
    public float vidaRestaurada = 100f; // Quantidade de vida a ser restaurada
    public float tempoParaReativarInput = 5f; // Tempo em segundos para reativar o InputManager
    private bool podeInteragir = false;
    private bool interacaoAtivada = true; // Controle de interação
    public Animator animator;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && interacaoAtivada)
        {
            podeInteragir = true;
            if (Input.GetKeyDown(KeyCode.E) && !IsGamePaused())
            {
                PlayerManager playerManager = other.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.RestaurarVida(vidaRestaurada);

                    // Desativar o componente InputManager no player
                    InputManager inputManager = other.GetComponent<InputManager>();
                    if (inputManager != null)
                    {
                        inputManager.ResetInputs(); // Zerar valores do InputManager
                        inputManager.enabled = false;
                        StartCoroutine(ReativarInputManager(inputManager));
                    }

                    // Acionar a animação de fechamento
                    if (animator != null)
                    {
                        animator.SetTrigger("Fechar");
                    }

                    // Desativar a interação até o reset
                    interacaoAtivada = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
        }
    }

    private void OnGUI()
    {
        if (podeInteragir && !IsGamePaused() && interacaoAtivada)
        {
            Vector3 posicaoCaixa = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
            Vector2 posicaoTexto = new Vector2(posicaoCaixa.x - 50, Screen.height - posicaoCaixa.y);

            GUIStyle stylez = new GUIStyle();
            stylez.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(posicaoTexto.x, posicaoTexto.y, 200, 30), "Pressione E");
        }
    }

    private bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    private IEnumerator ReativarInputManager(InputManager inputManager)
    {
        yield return new WaitForSeconds(tempoParaReativarInput);
        inputManager.enabled = true;
        Debug.Log("InputManager reativado!");

        ResetHealthBox();
        interacaoAtivada = true; // Permitir nova interação
    }

    private void ResetHealthBox()
    {
        if (animator != null)
        {
            animator.SetTrigger("Abrir");
        }
    }
}
