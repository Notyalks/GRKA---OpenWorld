using UnityEngine;

public class BossQuebravel : MonoBehaviour
{
    public float vidaMaxima = 100f;    // Vida máxima do boss
    private float vidaAtual;           // Vida atual do boss

    public BossLife bossLifeUI;        // Referência ao script BossLife
    public GameObject destructionParticles; // Sistema de partículas para a destruição
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem para completar a missão
    public CaveTutorialManager tutorialManager; // Referência ao CaveTutorialManager para informar a conclusão do boss

    private bool bossAlive = true; // Flag para controlar se o boss está vivo

    void Start()
    {
        vidaAtual = vidaMaxima;
        bossLifeUI.BossColocarVidaMaxima(vidaMaxima);   // Inicializa o slider com a vida máxima
    }

    void TomarDano(float quantidadeDano)
    {
        if (!bossAlive)
            return; // Se o boss já estiver morto, não processa mais dano

        vidaAtual -= quantidadeDano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0f, vidaMaxima);   // Garante que a vida não seja menor que zero nem maior que a vida máxima

        bossLifeUI.BossAlterarVida(vidaAtual);   // Atualiza o slider com a nova vida atual

        if (vidaAtual <= 0f)
        {
            Morrer();   // Função para lidar com a morte do boss
        }
    }

    void Morrer()
    {
        if (!bossAlive)
        {
            Debug.Log("Tentativa de morte do boss quando já está morto.");
            return;
        }

        bossAlive = false;
        Debug.Log("Boss morreu!");

        if (destructionParticles != null)
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        // Marca a missão como completa antes de notificar o tutorialManager
        if (objectiveSystem != null)
        {
            objectiveSystem.CompleteObjective("Destrua o Cristal mãe");
        }

        // Notifica o tutorialManager que o boss foi derrotado
        if (tutorialManager != null)
        {
            tutorialManager.OnBossDefeated();
        }

        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("bafo"))
        {
            TomarDano(1f);   // Chama o método TomarDano ao ser atingido pela partícula
        }
    }
}
