using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cyclops : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    Animator anim;
    Rigidbody rb;
    public float attackRange;
    public GameObject colliderAtk;
    public bool atacou = false;
    public bool morreu = false;
    public int vidaAtual;
    private int vidaTotal = 200;
    [SerializeField] private BarraDeVida barraDeVida;

    [SerializeField] private float timer = 5f;
    private float bulletTime;
    public GameObject enemyBullet;
    public Transform spawnPoint;
    public float speedArrow; 

    enum State
    {
        IDLE,
        ATACK,
        SHOOT,
        PATROL,
        DAMAGE,
        DIE,
    }


    [SerializeField]
    State state = State.IDLE;
    State oldState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        vidaAtual = vidaTotal;
        barraDeVida.AlterarBarraDeVida(vidaAtual, vidaTotal);
        state = State.IDLE;
    }

    void Update()
    {
        if (oldState != state)
        {
            oldState = state;
            switch (state)
            {
                case State.IDLE:
                    StartCoroutine(Idle());
                    break;
                case State.SHOOT:
                    StartCoroutine(Shoot());
                    break;
                case State.ATACK:
                    StartCoroutine(Atack());
                    break;
                case State.DIE:
                    StartCoroutine(Dead());
                    break;
                case State.PATROL:
                    StartCoroutine(Patrol());
                    break;  
            }
        }
    }


    IEnumerator Idle()
    {
        while (!target && state == State.IDLE)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            anim.SetFloat("Turn", 0);
            yield return new WaitForSeconds(3f);
            Debug.Log("Switching to Patrol state");
            state = State.PATROL;
            anim.SetFloat("Speed", agent.velocity.magnitude);
            anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));
        }
        
    }

    IEnumerator Shoot()
    {

        while (target && state == State.SHOOT)
        {
            if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE)
            {
                state = State.ATACK;
                agent.isStopped = true;
            }
            else
            {
                Debug.Log("entrou no ataque 2");
                anim.SetBool("atk2", true);
                ShootAtPlayer();
                yield return new WaitForSeconds(1f);
                anim.SetBool("atk2", false);
            }
        }
    }

    IEnumerator Atack()
    {
        anim.SetBool("atk1", true);
        colliderAtk.SetActive(true);
        atacou = true;
        yield return new WaitForSeconds(0.7f);
        colliderAtk.SetActive(false);
        atacou = false;
        anim.SetBool("atk1", false);
        agent.isStopped = false;
        state = State.IDLE;
    }

    IEnumerator Dead()
    {
        if (morreu)
        {
            float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            morreu = false;
            agent.isStopped = true;
            anim.SetBool("die", true);
            rb.constraints = RigidbodyConstraints.FreezePosition;
            yield return new WaitForSeconds(animDuration + 1);
            Destroy(this.gameObject);
        }
    }

    IEnumerator Patrol()
    {
        Debug.Log("Patrulhando");
        agent.isStopped = false;
        while (!target && state == State.PATROL)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
            anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));
            RandomPlacesToGO();
            yield return new WaitForSeconds(5);
        }
        if(target)
        {
            state = State.SHOOT;
        }

    }

    void RandomPlacesToGO()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 20;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 20, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0) return;

        bulletTime = timer;
        GameObject arrow = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        arrowRb.AddForce(arrowRb.transform.forward * speedArrow);
        Destroy(arrow, 7f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (state == State.DIE)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            target = other.transform;
            state = State.SHOOT;
        }

        if (other.gameObject.CompareTag("Fire"))
        {
            AplicarDano(5);
        }
    }



    public void OnTriggerExit(Collider other)
    {
        if (state == State.DIE)
            return;

        if (atacou)
        {
            return;
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                target = null;
                state = State.IDLE;
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            AplicarDano(5);
        }
    }

    private void AplicarDano(int dano)
    {
        if (state == State.DIE)
            return;

        vidaAtual -= 5;
        barraDeVida.AlterarBarraDeVida(vidaAtual, vidaTotal);

        if (vidaAtual <= 0)
        {
            state = State.DIE;
            morreu = true;

        }
    }

}
