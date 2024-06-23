using UnityEngine;

public class BossQuebravel : MonoBehaviour
{
    public float vidaMaxima = 100f;    // Vida máxima do boss
    private float vidaAtual;           // Vida atual do boss

    public BossLife bossLifeUI;        // Referência ao script BossLife

    void Start()
    {
        vidaAtual = vidaMaxima;
        bossLifeUI.BossColocarVidaMaxima(vidaMaxima);   // Inicializa o slider com a vida máxima
    }

    void TomarDano(float quantidadeDano)
    {
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
        // Implemente as ações de morte do boss aqui
        Debug.Log("Boss morreu!");
        Destroy(gameObject);   // Por exemplo, destrói o objeto do boss
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("bafo"))
        {
            TomarDano(1f);   // Chama o método TomarDano ao ser atingido pela partícula
        }
    }
}
