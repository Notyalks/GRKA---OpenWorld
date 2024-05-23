using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossFinal : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    Animator anim;
    Rigidbody rb;
    public ParticleSystem particle;
    public PlayerManager playerManager;
    public HealthBar healthBar;
    public float attackRange;
    public GameObject collAtkRight;
    public GameObject collAtkleft;
    public GameObject collAtkDown;
    public bool atacou = false;
    public bool morreu = false;
    public int vidaAtual;
    public int vida = 60000;
    public int fase = 1;
    [SerializeField] private BossLife lifebar;

    public GameObject bullet;
    public Transform bulletPos;

    public GameObject[] miniBossPrefabs;
    public Transform spawnPoint;

    enum State
    {
        IDLE,
        BERSERK,
        ATACK,
        BAFO,
        JUMP,
        ATACK2,
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
        target = null;
        state = State.IDLE;
        StartCoroutine(Idle());
        vidaAtual = vida;
        vida = 60000;
        lifebar.BossColocarVidaMaxima(vida);
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
                case State.BAFO:
                    StartCoroutine(Bafo());
                    break;
                case State.JUMP:
                    StartCoroutine(Jump());
                    break;
                case State.ATACK2:
                    StartCoroutine(Atack2());
                    break;
                case State.DIE:
                    StartCoroutine(Dead());
                    break;
                case State.SHOOT:
                    StartCoroutine(Shoot());
                    break;
            }
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));
        
        if (state == State.SHOOT && target != null)
        {
            agent.transform.LookAt(target.position);
        }

        if(vida <= 60000 && vida > 50000)
        {
            fase = 1;
        }
        if(vida <= 50000 && vida > 40000)
        {
            fase = 2;
        }
        if(vida <= 40000 && vida > 30000)
        {
            fase = 3;
        }
        if(vida <= 30000 && vida > 20000)
        {
            fase = 4;
        }
        if(vida <= 20000 && vida > 10000)
        {
            fase = 5;
        }
        if (vida <= 10000 && vida > 5000)
        {
            fase = 5;
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
            yield return new WaitForSeconds(7);
            SpawnMiniBoss();
            yield return new WaitForSeconds(3f);
        }

    }

    void SpawnMiniBoss()
    {

        int randomIndex = Random.Range(0, miniBossPrefabs.Length);
        GameObject selectedPrefab = miniBossPrefabs[randomIndex];
        Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    IEnumerator Berserk()
    {
        
        while (target && state == State.BERSERK)
        {
            if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE && fase == 1)
            {
                state = State.ATACK;
                agent.isStopped = true;
            }
            else if ((Vector3.Distance(transform.position, target.position) <= attackRange + 20) && state != State.DIE && fase == 2)
            {
                agent.stoppingDistance = 10;
                anim.SetBool("punch", false);
                state = State.BAFO;
                agent.isStopped = true;
            }
            else if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE && fase == 3)
            {
                anim.SetBool("punch", false);
                state = State.JUMP;
                agent.isStopped = true;
            }
            else if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE && fase == 4)
            {
                anim.SetBool("punch", false);
                state = State.ATACK2;
                agent.isStopped = true;
            }
            else if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE && fase == 5)
            {
                anim.SetBool("punch", false);
                state = State.SHOOT;
                agent.isStopped = true;
            }
            else
            {
                anim.SetBool("punch", false);
                agent.isStopped = false;
                agent.SetDestination(target.position);
                anim.SetFloat("Speed", agent.velocity.magnitude);
                anim.SetFloat("Turn", Vector3.Dot(agent.velocity.normalized, transform.forward));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    IEnumerator Bafo()
    {
        agent.isStopped = true;
        
        anim.SetBool("bafo", true);
        yield return new WaitForSeconds(4f);
        anim.SetBool("bafo", false);
        agent.isStopped = false;
        state = State.BERSERK;
    }

    IEnumerator Jump()
    {
        agent.isStopped = true;
        anim.SetBool("jump", true);
        yield return new WaitForSeconds(2f);
        collAtkDown.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        collAtkDown.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("jump", false);
        agent.isStopped = false;
        state = State.BERSERK;
    }


    IEnumerator Shoot()
    {
        while (target && state == State.SHOOT)
        {
            if ((Vector3.Distance(transform.position, target.position) <= attackRange) && state != State.DIE && fase == 5)
            {
                anim.SetBool("fire", true);
                yield return new WaitForSeconds(1.40f);
                Instantiate(bullet, bulletPos.position, Quaternion.identity);
                yield return new WaitForSeconds(1f);
                Instantiate(bullet, bulletPos.position, Quaternion.identity);
                anim.SetBool("fire", false);
            }
        }

    }

    IEnumerator Atack()
    {
        anim.SetBool("punch", true);
        collAtkleft.SetActive(true);
        atacou = true;
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("punch", false);
        collAtkleft.SetActive(false);
        atacou = false;
        agent.isStopped = false;
        state = State.BERSERK;
    }

    IEnumerator Atack2()
    {
        anim.SetBool("soco", true);
        yield return new WaitForSeconds(1f);
        collAtkRight.SetActive(true);
        atacou = true;
        yield return new WaitForSeconds(1.7f);
        anim.SetBool("soco", false);
        collAtkRight.SetActive(false);
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
            AplicarDano(50);
        }
    }

    private void OnParticleTrigger()
    {
       if (CompareTag("Player"))
        {
            playerManager.vida -= 0.05f;
            healthBar.AlterarVida(playerManager.vida);
        } 
    }
    private void OnParticleCollision(GameObject other)
    {
        if (CompareTag("Player"))
        {
            playerManager.vida -= 1f;
            healthBar.AlterarVida(playerManager.vida);
        }
    }
    private void AplicarDano(int dano)
    {
        if (state == State.DIE)
            return;

        vida -= 50;
        lifebar.BossAlterarVida(vida);

        if (vida <= 0)
        {
            state = State.DIE;
            morreu = true;

        }
    }


    
}
