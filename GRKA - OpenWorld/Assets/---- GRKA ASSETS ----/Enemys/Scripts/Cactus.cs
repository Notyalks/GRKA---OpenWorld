using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Cactus : MonoBehaviour
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
    private int vidaTotal = 100;
    [SerializeField] private BarraDeVida barraDeVida;


    enum State
    {
        IDLE,
        ATACK,
        BERSERK,
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
        state = State.IDLE;
        vidaAtual = vidaTotal;
        barraDeVida.AlterarBarraDeVida(vidaAtual, vidaTotal);
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
                case State.BERSERK:
                    StartCoroutine(Berserk());
                    break;
                case State.ATACK:
                    StartCoroutine(Atack());
                    break;
                case State.DIE:
                    StartCoroutine(Dead());
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
            yield return new WaitForSeconds(0.1f);
        }

    }

    IEnumerator Berserk()
    {

        while (target && state == State.BERSERK)
        {
            if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE)
            {
                state = State.ATACK;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                anim.SetFloat("Speed", agent.velocity.magnitude);
                anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));
                yield return new WaitForSeconds(0.1f);
            }
        }

        //  state = State.IDLE;

    }

    IEnumerator Atack()
    {
        anim.SetBool("atk1", true);
        colliderAtk.SetActive(true);
        atacou = true;
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("atk1", false);
        colliderAtk.SetActive(false);
        atacou = false;
        agent.isStopped = false;
        state = State.BERSERK;
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

    public void OnTriggerEnter(Collider other)
    {
        if (state == State.DIE)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            target = other.transform;
            StopAllCoroutines();
            state = State.BERSERK;
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

    //IEnumerator Patrol()
    //{
    //    Debug.Log("Patrol");
    //    while (!target && state == State.PATROL)
    //    {
    //        yield return new WaitForSeconds(1);
    //        RandomPlacesToGO();
    //        yield return new WaitForSeconds(5);
    //    }

    //}


    //void RandomPlacesToGO()
    //{
    //    Vector3 randomDirection = Random.insideUnitSphere * 10;
    //    randomDirection += transform.position;
    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
    //    Vector3 finalPosition = hit.position;
    //    agent.SetDestination(finalPosition);
    //}
}
