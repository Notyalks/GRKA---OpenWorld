using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    Rigidbody rb;
    public bool atacou = false;
    public bool morreu = false;
    public int vidaAtual;
    private int vidaTotal = 100;
    public float attackRange;
    public Transform target;
    [SerializeField] private BarraDeVida barraDeVida;
    public GameObject body1;
    public GameObject body2;
    public Image back;
    public Image bar;

    enum State
    {
        IDLE,
        ATACK,
        BERSERK,
        PATROL,
        BLINK,
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
        target = null;
        state = State.IDLE;
        StartCoroutine(Idle());
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
                case State.ATACK:
                    StartCoroutine(Atack());
                    break;
                case State.DIE:
                    StartCoroutine(Dead());
                    break;
                case State.PATROL:
                    StartCoroutine(Patrol());
                    break;
                case State.BERSERK:
                    StartCoroutine(Berserk());
                    break;
                case State.BLINK:
                    StartCoroutine(Blink());
                    break;
            }
        }


    }


    IEnumerator Idle()
    {
        anim.SetBool("atk1", false);
        while (!target && state == State.IDLE)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            anim.SetFloat("Turn", 0);
            body1.gameObject.SetActive(false);
            body2.gameObject.SetActive(false);
            back.gameObject.SetActive(false);
            bar.gameObject.SetActive(false);
            yield return new WaitForSeconds(3f);
            state = State.PATROL;
        }

    }

    IEnumerator Patrol()
    {
        anim.SetBool("atk1", false);
        agent.isStopped = false;
        while (!target && state == State.PATROL)
        {
            anim.SetBool("atk1", false);
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2);
            RandomPlacesToGO();
            yield return new WaitForSeconds(2);
        }

    }


    IEnumerator Berserk()
    {
        while (target && state == State.BERSERK)
        {
            if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE)
            {
                body1.gameObject.SetActive(true);
                body2.gameObject.SetActive(true);
                back.gameObject.SetActive(true);
                bar.gameObject.SetActive(true);
                state = State.ATACK;
                agent.isStopped = true;
            }
            else
            {
                anim.SetBool("atk1", false);
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
        body1.gameObject.SetActive(true);
        body2.gameObject.SetActive(true);
        back.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);
        anim.SetBool("atk1", true);
        atacou = true;
        yield return new WaitForSeconds(0.65f);
        
        yield return new WaitForSeconds(0.01f);
        body1.gameObject.SetActive(false);
        body2.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        anim.SetBool("atk1", false);
        atacou = false;
        agent.isStopped = false;
        state = State.BERSERK;
    }

    IEnumerator Dead()
    {
        if (morreu)
        {
            body1.gameObject.SetActive(true);
            body2.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
            bar.gameObject.SetActive(true);
            float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            morreu = false;
            agent.isStopped = true;
            anim.SetBool("die", true);
            rb.constraints = RigidbodyConstraints.FreezePosition;
            yield return new WaitForSeconds(animDuration + 2);
            Destroy(this.gameObject);
        }
    }

    IEnumerator Blink()
    {
        body1.gameObject.SetActive(true);
        body2.gameObject.SetActive(true);
        back.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);
        anim.SetBool("damage", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("damage", false);
        body1.gameObject.SetActive(false);
        body2.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        state = State.BERSERK;  
        if(vidaAtual <= 0)
        {
            Debug.Log("morreufi");
            state = State.DIE;
            morreu = true;
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
            state = State.BLINK;
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

        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
            state = State.IDLE;
        }



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            AplicarDano(5);
            StopAllCoroutines();
            state = State.BLINK;
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
            Debug.Log("morreufi");
            state = State.DIE;
            morreu = true;
        }
    }


    void RandomPlacesToGO()
    {

        Vector3 randomDirection = Random.insideUnitSphere * 30;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 30, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);

    }
}
