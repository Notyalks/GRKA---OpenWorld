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
    private int vidaTotal = 100;
    [SerializeField] private BarraDeVida barraDeVida;

    public GameObject bullet;
    public Transform[] bulletPos;
    
    enum State
    {
        IDLE,
        ATACK,
        BERSERK,
        PATROL,
        SHOOT,
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
                case State.SHOOT:
                    StartCoroutine(Shoot());
                    break;
            }
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));

        if(state == State.SHOOT && target != null)
        {
            agent.transform.LookAt(target.position);
        }
    }


    IEnumerator Idle()
    {
        
        while (!target && state == State.IDLE)
        {
            anim.SetBool("atk2", false);
            anim.SetBool("atk1", false);
            Debug.Log("passouotempo");
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            anim.SetFloat("Turn", 0);
            yield return new WaitForSeconds(3f);
            state = State.PATROL;
        }

    }

    IEnumerator Patrol()
    {
        agent.isStopped = false;
        Debug.Log("Patrol");
        while (!target && state == State.PATROL)
        {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2);
            RandomPlacesToGO();
            yield return new WaitForSeconds(2);
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
                anim.SetBool("atk2", true);
                yield return new WaitForSeconds(0.5f);
                Instantiate(bullet, bulletPos[0].position, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
                Instantiate(bullet, bulletPos[1].position, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
                Instantiate(bullet, bulletPos[2].position, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
                Instantiate(bullet, bulletPos[3].position, Quaternion.identity);
                yield return new WaitForSeconds(0.8f);

                //if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE)
                //{
                //    anim.SetBool("atk2", false);
                //    state = State.ATACK;
                //    agent.isStopped = true;
                //}
            }
        }
        
    }

    IEnumerator Atack()
    {
        anim.SetBool("atk2", false);
        anim.SetBool("atk1", true);
        yield return new WaitForSeconds(1f);
        colliderAtk.SetActive(true);
        atacou = true;
        yield return new WaitForSeconds(1.7f);
        anim.SetBool("atk1", false);
        colliderAtk.SetActive(false);
        atacou = false;
        agent.isStopped = false;
        state = State.SHOOT;
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
            yield return new WaitForSeconds(animDuration + 2);
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

        //if (atacou)
        //{
        //    return;
        //}
        
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
