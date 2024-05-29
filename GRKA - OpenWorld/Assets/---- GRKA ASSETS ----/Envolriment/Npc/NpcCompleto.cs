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
    public bool isInteractable = true;
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
    public List<Dialogo> dialogos = new List<Dialogo>();

    private bool podeInteragir = false;
    private int indiceDialogo = 0;
    private int indiceFalaAtual = 0;

    private InputManager inputManager;

    private static NpcCompleto npcInteragindo;

    private Quaternion rotacaoInicial;
    private Quaternion rotacaoAntesInteracao; // Nova variável para armazenar a rotação antes da interação

    [Header("Configurações de Diálogo")]
    public bool dialogosAleatorios = false;

    void Start()
    {
        Painel.SetActive(false);
        agente = GetComponent<NavMeshAgent>();
        inputManager = FindObjectOfType<InputManager>();

        if (isPatrolling)
        {
            SetDestino();
        }

        rotacaoInicial = transform.rotation;
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

        if (isInteractable && podeInteragir && Input.GetKeyDown(KeyCode.E) && !IsGamePaused())
        {
            Interact();
        }

        if (Painel.activeSelf && Input.GetMouseButtonDown(0) && !IsGamePaused())
        {
            ProximoDialogo();
        }

        if (Painel.activeSelf && jogador != null && npcInteragindo == this)
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
        if (isInteractable && other.CompareTag("Player"))
        {
            podeInteragir = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isInteractable && other.CompareTag("Player"))
        {
            podeInteragir = false;
            Painel.SetActive(false);
            ResetDialogo();

            if (isPatrolling)
            {
                agente.enabled = true;
                SetDestino();
            }

            // Voltar para a rotação antes da interação
            transform.rotation = rotacaoAntesInteracao;
        }
    }

    void OnGUI()
    {
        if (isInteractable && podeInteragir && !Painel.activeSelf && !IsGamePaused())
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

            // Armazena a rotação antes da interação
            rotacaoAntesInteracao = transform.rotation;

            indiceFalaAtual = 0;
            Painel.SetActive(true);
            nomeDoNPCUI.text = nomeDoNPC;
            ExibirDialogo();
            agente.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            npcInteragindo = this;
        }
    }

    public void ProximoDialogo()
    {
        indiceFalaAtual++;

        if (indiceFalaAtual >= dialogos[indiceDialogo].falas.Length)
        {
            indiceFalaAtual = 0;
            Painel.SetActive(false);
            ResetDialogo();

            if (dialogosAleatorios)
            {
                indiceDialogo = Random.Range(0, dialogos.Count);
            }
            else
            {
                indiceDialogo++;
                if (indiceDialogo >= dialogos.Count)
                {
                    indiceDialogo = 0;
                }
            }

            if (isPatrolling)
            {
                agente.enabled = true;
                SetDestino();
            }

            npcInteragindo = null;

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

        if (npcInteragindo == this)
        {
            npcInteragindo = null;
        }
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
