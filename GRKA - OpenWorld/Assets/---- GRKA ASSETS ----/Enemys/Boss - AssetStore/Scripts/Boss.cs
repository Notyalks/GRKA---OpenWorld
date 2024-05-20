using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public int rutina;
    public float cronometro;
    public float time_rutinas;
    public Animator ani;
    public Quaternion angulo;
    public float grado;
    public GameObject target;
    public bool atacando;
    public RangoBoss rango;
    public float speed;
    public GameObject[] hit;
    public int hit_select;


    public bool lanza_llamas;
    public List<GameObject> pool = new List<GameObject>();
    public GameObject fire;
    public GameObject cabeza;
    private float cronometro2;

    public float jump_distance;
    public bool direction_Skill;

    public GameObject fire_ball;
    public GameObject point;
    public List<GameObject> pool2 = new List<GameObject>();

    public int fase = 1;
    public float HP_Min;
    public float HP_Max;
    public Image barra;
    public AudioSource musica;
    public bool muerto;


    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("ChaT");
    }

    public void Comportamiento_Boss()
    {
        if(Vector3.Distance(transform.position, target.transform.position) < 15)
        {
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            point.transform.LookAt(target.transform.position);
            musica.enabled = true;

            if(Vector3.Distance(transform.position, target.transform.position) > 1 && !atacando)
            {
                switch (rutina)
                {
                    case 0:
                        Debug.Log("case0");
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                       // transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2 * Time.deltaTime);
                        ani.SetBool("walk", true);
                        ani.SetBool("run", false);

                       if(transform.rotation == rotation)
                       // if (Quaternion.Angle(transform.rotation, rotation) < 5f)
                        {
                            Debug.Log("Anda");
                            transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        }

                        ani.SetBool("attack", false);

                        //cronometro += 1 * Time.deltaTime;
                        cronometro += Time.deltaTime;

                        if (cronometro > time_rutinas)
                        {
                            rutina = Random.Range(0, 5);
                            cronometro = 0;
                        }
                    break;


                    case 1:
                        Debug.Log("case1");
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                        ani.SetBool("walk", false);
                        ani.SetBool("run", true);

                        if(transform.rotation == rotation)
                        {
                            transform.Translate(Vector3.forward * speed * 2 * Time.deltaTime);
                        }

                        ani.SetBool("attack", false);
                    break;


                    case 2:
                        Debug.Log("case2");
                        ani.SetBool("walk", false);
                        ani.SetBool("run", false);
                        ani.SetBool("attack", true);
                        ani.SetFloat("skills", 0.8f);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                        rango.GetComponent<CapsuleCollider>().enabled = false;
                    break;


                    case 3:
                        Debug.Log("case3");
                        if (fase == 2)
                        {
                            jump_distance += 1 * Time.deltaTime;
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 0.6f);
                            hit_select = 3;
                            rango.GetComponent<CapsuleCollider>().enabled = false;

                            if (direction_Skill)
                            {
                                if(jump_distance < 1f)
                                {
                                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                                }

                                transform.Translate(Vector3.forward * 8 * Time.deltaTime);
                            }
                        }
                        else
                        {
                            rutina = 0;
                            cronometro = 0;
                        }

                    break;


                    case 4:
                        Debug.Log("case4");
                        if (fase == 2)
                        {
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 1);
                            rango.GetComponent<CapsuleCollider>().enabled = false;
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.5f);
                        }
                        else
                        {
                            rutina = 0;
                            cronometro = 0;
                        }
                    break;
                }
            }
        }
    }


    public void Final_Ani()
    {
        rutina = 0;
        ani.SetBool("attack", false);
        atacando = false;
        rango.GetComponent <CapsuleCollider>().enabled = false;
        lanza_llamas = false;
        jump_distance = 0;
        direction_Skill = false;
    }

    public void Direction_Attack_Start()
    {
        direction_Skill = true;
    }

    public void Direction_Attack_Final()
    {
        direction_Skill = false;
    }

    public void ColliderWeaponTrue()
    {
        hit[hit_select].GetComponent<SphereCollider>().enabled = true;
    }

    public void ColliderWeaponFalse()
    {
        hit[hit_select].GetComponent<SphereCollider>().enabled = false;
    }


    public GameObject GetBala()
    {
        for(int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }

        }
        GameObject obj = Instantiate(fire, cabeza.transform.position, cabeza.transform.rotation) as GameObject;
        pool.Add(obj);
        return obj; 
    }

    public void LanzaLlamas_Skill()
    {
        cronometro2 += 1 * Time.deltaTime;
        if(cronometro2 > 0.1f)
        {
            GameObject obj = GetBala();
            obj.transform.position = cabeza.transform.position;
            obj.transform.rotation = cabeza.transform.rotation;
            cronometro2 = 0;
        }
    }

    public void Start_Fire()
    {
        lanza_llamas = true;
    }

    public void Stop_Fire()
    {
        lanza_llamas = false;
    }

    public GameObject Get_Fire_Ball()
    {
        for (int i = 0; i < pool2.Count; i++)
        {
            if (!pool2[i].activeInHierarchy)
            {
                pool2[i].SetActive(true);
                return pool2[i];
            }

        }
        GameObject obj = Instantiate(fire_ball, point.transform.position, point.transform.rotation) as GameObject;
        pool2.Add(obj);
        return obj;
    }

    public void Fire_Ball_Skill()
    {
        GameObject obj = Get_Fire_Ball();
        obj.transform.position = point.transform.position;
        obj.transform.rotation = point.transform.rotation;
    }

    public void Vivo()
    {
        if (HP_Min < 500)
        {
            fase = 2;
            time_rutinas = 1;
        }
        Comportamiento_Boss();

        if(lanza_llamas)
        {
            LanzaLlamas_Skill();
        }
    }

    void Update() 
    {
        barra.fillAmount = HP_Min / HP_Max;

        if(HP_Min > 0)
        {
            Vivo();
        }
        else
        {
            if (!muerto)
            {
                ani.SetTrigger("dead");
                musica.enabled = false;
                muerto = true;
            }
        }

    }
}
