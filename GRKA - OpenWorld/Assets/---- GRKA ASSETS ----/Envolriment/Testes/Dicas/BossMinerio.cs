using UnityEngine;

public class BossQuebravel : MonoBehaviour
{
    public float vidaMaxima = 100f;    // Vida m�xima do boss
    private float vidaAtual;           // Vida atual do boss

    public BossLife bossLifeUI;        // Refer�ncia ao script BossLife

    void Start()
    {
        vidaAtual = vidaMaxima;
        bossLifeUI.BossColocarVidaMaxima(vidaMaxima);   // Inicializa o slider com a vida m�xima
    }

    void TomarDano(float quantidadeDano)
    {
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
        // Implemente as a��es de morte do boss aqui
        Debug.Log("Boss morreu!");
        Destroy(gameObject);   // Por exemplo, destr�i o objeto do boss
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("bafo"))
        {
            TomarDano(1f);   // Chama o m�todo TomarDano ao ser atingido pela part�cula
        }
    }
}
