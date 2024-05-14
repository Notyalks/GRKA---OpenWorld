using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Cactus : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    public Animator anim;
    public SkinnedMeshRenderer render;
    public float DistanceToAttack = 3;

    public enum States
    {
        idle,
        atacking,
        pursuit,
        damage,
        dead
    }

    public States state;

    void Start()
    { 
        StartCoroutine("IdleState");
    }

    void StateMachine(States _state)
    {
        state = _state;
        switch (state)
        {
            case States.idle:
                StartCoroutine("IdleState");
                break;
            case States.pursuit:
                StartCoroutine("PursuitState");
                break;
            case States.atacking:
                StartCoroutine("AttackState");
                break;
            case States.damage:
                StartCoroutine("Damage");
                break;
            case States.dead:
                StartCoroutine("Dead");
                break;
        }
    }

    IEnumerator IdleState()
    {
        while (!target)
        {
            yield return new WaitForSeconds(1);
            
        }
    }

    IEnumerator PursuitState()
    {
        Debug.Log("entrou");
        agent.isStopped = false;
        anim.SetBool("atk1", false);
        anim.SetBool("damage", false);
        anim.SetFloat("velocidade", agent.velocity.magnitude);
        agent.SetDestination(target.position);
        yield return new WaitForSeconds(0.1f);
        //if (Vector3.Distance(transform.position, target.transform.position) < DistanceToAttack)
        //{
        //    StateMachine(States.atacking);
        //}
        //else if (Vector3.Distance(transform.position, target.transform.position) > DistanceToAttack * 5)
        //{
        //    StateMachine(state = States.idle);
        //}
        //else
        //{
        //    StateMachine(States.pursuit);
        //}
    }

    IEnumerator AttackState()
    {
        agent.isStopped = true;
        anim.SetBool("atk1", true);
        anim.SetBool("damage", false);
        yield return new WaitForSeconds(0.1f);
        if (Vector3.Distance(transform.position, target.transform.position) > 4)
        {
            StateMachine(States.pursuit);
        }
        else
        {
            StateMachine(States.atacking);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            target = other.transform;
            StopAllCoroutines();
            state = States.pursuit;
        }
    }

    internal void Damage()
    {
        StateMachine(States.damage);
    }

    IEnumerator DamageState()
    {
        agent.isStopped = true;
        anim.SetBool("Damage", true);
        for (int i = 0; i < 4; i++)
        {
            render.material.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(0.05f);
            render.material.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(0.05f);
        }
        StateMachine(States.pursuit);
    }

    public void Dead()
    {
        StopAllCoroutines();
        StateMachine(States.dead);

    }
    
    IEnumerator DeadState()
    {
        agent.isStopped = true;
        anim.SetBool("Attack", false);
        anim.SetBool("Dead", true);
        anim.SetBool("Damage", false);
        yield return new WaitForSeconds(0.05f);
    }
}
