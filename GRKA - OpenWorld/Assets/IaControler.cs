using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class IaControler : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    Animator anim;


    enum State
    {
        Idle,
        Patrol,
        Berserk
    }
    [SerializeField]
    State state = State.Idle;
    State oldState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(player.position);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (oldState != state)
        {
            oldState = state;
            StopAllCoroutines();
            switch (state)
            {
            case State.Idle:
                 StartCoroutine(Idle());
                 break;
            case State.Patrol:
                 StartCoroutine(Patrol());
                 break;
            case State.Berserk:
                 StartCoroutine(Bersek());
                 break;
            }

        }
    }

        IEnumerator Bersek()
        {
            while (player)
            {
                anim.SetFloat("Speed", agent.velocity.magnitude);
                anim.SetFloat("Direction", Vector3.Dot(agent.velocity.normalized, transform.forward));
                agent.SetDestination(player.position);
                agent.speed = 10;
                yield return new WaitForSeconds(1);
            }
            state = State.Idle;
        }
        IEnumerator Idle()
        {
            while (!player)
            {
                yield return new WaitForSeconds(5);
                state = State.Patrol;
            }
            state = State.Berserk;
        }
    
    IEnumerator Patrol()
    {
        while (!player)
        {
            yield return new WaitForSeconds(1);
            RandomPlacesToGo();
            yield return new WaitForSeconds(5);
        }
            state = State.Berserk;
    }

    public void RandomPlacesToGo()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;  

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }
}
