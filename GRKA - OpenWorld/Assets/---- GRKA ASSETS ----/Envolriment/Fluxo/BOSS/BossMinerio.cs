using UnityEngine;

public class BossQuebravel : MonoBehaviour
{
    public float vidaMaxima = 100f;    // Vida m�xima do boss
    private float vidaAtual;           // Vida atual do boss

    public BossLife bossLifeUI;        // Refer�ncia ao script BossLife
    public GameObject destructionParticles; // Sistema de part�culas para a destrui��o
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem para completar a miss�o
    public CaveTutorialManager tutorialManager; // Refer�ncia ao CaveTutorialManager para informar a conclus�o do boss

    private bool bossAlive = true; // Flag para controlar se o boss est� vivo

    void Start()
    {
        vidaAtual = vidaMaxima;
        bossLifeUI.BossColocarVidaMaxima(vidaMaxima);   // Inicializa o slider com a vida m�xima
    }

    void TomarDano(float quantidadeDano)
    {
        if (!bossAlive)
            return; // Se o boss j� estiver morto, n�o processa mais dano

        vidaAtual -= quantidadeDano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0f, vidaMaxima);   // Garante que a vida n�o seja menor que zero nem maior que a vida m�xima

        bossLifeUI.BossAlterarVida(vidaAtual);   // Atualiza o slider com a nova vida atual

        if (vidaAtual <= 0f)
        {
            Morrer();   // Fun��o para lidar com a morte do boss
        }
    }

    void Morrer()
    {
        if (!bossAlive)
        {
            Debug.Log("Tentativa de morte do boss quando j� est� morto.");
            return;
        }

        bossAlive = false;
        Debug.Log("Boss morreu!");

        if (destructionParticles != null)
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        // Marca a miss�o como completa antes de notificar o tutorialManager
        if (objectiveSystem != null)
        {
            objectiveSystem.CompleteObjective("Destrua o Cristal m�e");
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
            TomarDano(1f);   // Chama o m�todo TomarDano ao ser atingido pela part�cula
        }
    }
}
