using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NpcCompleto : MonoBehaviour
{
    [Header("Movimento do NPC")]
    public bool isPatrolling = true;
    public float tempoDeEspera = 1f;
    public Transform[] pontosDeDestino;

    private int indiceDestino = 0;
    private NavMeshAgent agente;
    private bool esperando = false;
    private float tempoInicioEspera;

    [Header("Interação do NPC")]
    public Transform jogador;
    public GameObject Painel;
    public string nomeDoNPC;
    public Text nomeDoNPCUI;

    public enum TipoDeDialogo { Introducao, Missao, Conclusao }

    [System.Serializable]
    public class Dialogo
    {
        public TipoDeDialogo tipo;
        [TextArea(3, 10)]
        public string[] falas;
    }

    public Text textoDoDialogoUI;
    public List<Dialogo> dialogos = new List<Dialogo>(); // Usamos uma lista para os diálogos

    private bool podeInteragir = false;
    private int indiceDialogo = 0; // Cada NPC tem seu próprio índice de diálogo
    private int indiceFalaAtual = 0;

    private InputManager inputManager;

    void Start()
    {
        Painel.SetActive(false);
        agente = GetComponent<NavMeshAgent>();
        inputManager = FindObjectOfType<InputManager>();

        if (isPatrolling)
        {
            SetDestino();
        }
    }

    void Update()
    {
        if (isPatrolling && !Painel.activeSelf)
        {
            if (esperando)
            {
                if (Time.time >= tempoInicioEspera + tempoDeEspera)
                {
                    esperando = false;
                    SetDestino();
                }
            }
            else
            {
                if (!agente.pathPending && agente.remainingDistance < 0.1f)
                {
                    esperando = true;
                    tempoInicioEspera = Time.time;
                }
            }
        }

        if (podeInteragir && Input.GetKeyDown(KeyCode.E) && !IsGamePaused())
        {
            Interact();
        }

        if (Painel.activeSelf && Input.GetMouseButtonDown(0) && !IsGamePaused())
        {
            ProximoDialogo();
        }

        if (Painel.activeSelf && jogador != null)
        {
            transform.LookAt(jogador);
        }
    }

    void SetDestino()
    {
        if (pontosDeDestino.Length > 0)
        {
            agente.SetDestination(pontosDeDestino[indiceDestino].position);
            indiceDestino = (indiceDestino + 1) % pontosDeDestino.Length;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
            Painel.SetActive(false);
            ResetDialogo();
            if (isPatrolling)
            {
                agente.enabled = true;
                SetDestino();
            }
        }
    }

    void OnGUI()
    {
        if (podeInteragir && !Painel.activeSelf && !IsGamePaused())
        {
            Vector3 posicaoNPC = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 6);
            Vector2 posicaoTexto = new Vector2(posicaoNPC.x - 50, Screen.height - posicaoNPC.y + 50);

            GUIStyle stylez = new GUIStyle();
            stylez.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(posicaoTexto.x, posicaoTexto.y, 200, 30), "Pressione E");
        }
    }

    public void Interact()
    {
        if (!Painel.activeSelf)
        {
            inputManager.ResetInputs();
            inputManager.enabled = false;

            indiceFalaAtual = 0;
            Painel.SetActive(true);
            nomeDoNPCUI.text = nomeDoNPC;
            ExibirDialogo();
            agente.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ProximoDialogo()
    {
        indiceFalaAtual++;

        // Verifica se é a última fala do diálogo atual
        if (indiceFalaAtual >= dialogos[indiceDialogo].falas.Length)
        {
            indiceFalaAtual = 0;
            Painel.SetActive(false);
            ResetDialogo();

            // Avança para o próximo diálogo
            indiceDialogo++;
            if (indiceDialogo >= dialogos.Count)
            {
                indiceDialogo = 0; // Reinicia o ciclo dos diálogos se necessário
            }

            if (isPatrolling)
            {
                agente.enabled = true;
                SetDestino();
            }

            return;
        }

        ExibirDialogo();
    }

    void ExibirDialogo()
    {
        if (indiceDialogo < dialogos.Count)
        {
            string textoDoDialogo = dialogos[indiceDialogo].falas[indiceFalaAtual];
            textoDoDialogoUI.text = textoDoDialogo;
            Debug.Log("Exibindo diálogo: " + textoDoDialogo);
        }
    }

    void ResetDialogo()
    {
        inputManager.enabled = true;

        indiceFalaAtual = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    public bool EstaEmDialogo()
    {
        return Painel.activeSelf;
    }
}
