using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWin : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 spawnPos;
    public bool hasSpawn;

    
    void Start()
    {
        hasSpawn = false;
    }

    void Update()
    {
        if(!hasSpawn && GameObject.FindGameObjectWithTag("Puzzle") == null)
        {
            Instantiate(prefab, spawnPos, Quaternion.identity);
            hasSpawn = true;
        }
    }
}
