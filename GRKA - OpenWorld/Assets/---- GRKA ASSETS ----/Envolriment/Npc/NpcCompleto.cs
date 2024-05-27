using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NpcCompleto : MonoBehaviour
{
    [Header("Movimento do NPC")]
    public Transform[] pontosDeDestino;
    public float tempoDeEspera = 1f;

    private int indiceDestino = 0;
    private NavMeshAgent agente;
    private bool esperando = false;
    private float tempoInicioEspera;

    [Header("Interação do NPC")]
    public GameObject Painel;
    public string nomeDoNPC;
    public Text nomeDoNPCUI;
    public Transform jogador; // Transform do jogador

    [System.Serializable]
    public class Dialogo
    {
        [TextArea(3, 10)]
        public string[] falas;
    }

    public Text textoDoDialogoUI;
    public Dialogo[] dialogos;

    private bool podeInteragir = false;
    private string textoDoDialogo;
    private int indiceDialogo = 0;
    private int indiceFalaAtual = 0;

    private InputManager inputManager;

    void Start()
    {
        Painel.SetActive(false);
        agente = GetComponent<NavMeshAgent>();
        inputManager = FindObjectOfType<InputManager>();
        SetDestino();
    }

    void Update()
    {
        if (!Painel.activeSelf)
        {
            if (esperando)
            {
                if (Time.time >= tempoInicioEspera + tempoDeEspera)
                {
                    esperando = false;
                    SetDestino();
                    Debug.Log("Espera terminada. Definindo novo destino.");
                }
            }
            else
            {
                if (!agente.pathPending && agente.remainingDistance < 0.1f)
                {
                    esperando = true;
                    tempoInicioEspera = Time.time;
                    Debug.Log("Chegou ao destino. Iniciando espera. Tempo de espera: " + tempoDeEspera + " segundos.");
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
            Debug.Log("Novo destino definido: " + pontosDeDestino[indiceDestino].name);
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
            agente.enabled = true;
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
            GUI.Label(new Rect(posicaoTexto.x, posicaoTexto.y, 200, 30), "Pressione E Para Conversar");
        }
    }

    public void Interact()
    {
        if (!Painel.activeSelf)
        {
            inputManager.ResetInputs(); // Resetar entradas
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
        if (indiceDialogo < dialogos.Length)
        {
            string[] falas = dialogos[indiceDialogo].falas;
            indiceFalaAtual++;

            if (indiceFalaAtual >= falas.Length)
            {
                indiceFalaAtual = 0;
                indiceDialogo++;
                Debug.Log("Avançando para o próximo diálogo. Diálogo atual: " + indiceDialogo);

                if (indiceDialogo >= dialogos.Length)
                {
                    Painel.SetActive(false);
                    ResetDialogo();
                    Debug.Log("Todos os diálogos foram exibidos. Reiniciando para o primeiro diálogo.");
                    agente.enabled = true;
                    return;
                }
            }

            ExibirDialogo();
        }
    }

    void ExibirDialogo()
    {
        if (indiceDialogo < dialogos.Length)
        {
            string[] falas = dialogos[indiceDialogo].falas;
            textoDoDialogo = falas[indiceFalaAtual];
            textoDoDialogoUI.text = textoDoDialogo;
            Debug.Log("Exibindo diálogo: " + textoDoDialogo);
        }
    }

    void ResetDialogo()
    {
        inputManager.ResetInputs(); // Resetar entradas
        inputManager.enabled = true;

        indiceDialogo = 0;
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
