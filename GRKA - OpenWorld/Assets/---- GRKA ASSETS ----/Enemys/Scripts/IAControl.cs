using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAControl : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    Animator anim;
    public float attackRange;

    enum State
    {
        IDLE,
        ATACK,
        BERSERK,
        DAMAGE,
        DIE,
        STATIC
    }
    [SerializeField]
    State state = State.IDLE;
    State oldState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
                case State.BERSERK:
                    StartCoroutine(Berserk());
                    break;
                case State.ATACK:
                    StartCoroutine(Atack());
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
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                state = State.ATACK;
                agent.isStopped = true;
                Debug.Log("entrou em atacar");
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
        Debug.Log("atacou");
        anim.SetBool("atk1", true);
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("atk1", false);
        agent.isStopped = false;
        state = State.BERSERK;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.transform;
            StopAllCoroutines();
            state = State.BERSERK;
            Debug.Log("Jogador entrou na área de gatilho do personagem.");
        }
    }



    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
            state = State.IDLE;
            Debug.Log("Jogador saiu da área de gatilho do personagem.");
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
