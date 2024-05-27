using UnityEngine;

public class HealthBox : MonoBehaviour
{
    public float vidaRestaurada = 100f; // Quantidade de vida a ser restaurada
    private bool podeInteragir = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
            if (Input.GetKeyDown(KeyCode.E) && !IsGamePaused())
            {
                PlayerManager playerManager = other.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.RestaurarVida(vidaRestaurada);
                    Debug.Log("Vida restaurada para o jogador!");
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
        if (podeInteragir && !IsGamePaused())
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
}
