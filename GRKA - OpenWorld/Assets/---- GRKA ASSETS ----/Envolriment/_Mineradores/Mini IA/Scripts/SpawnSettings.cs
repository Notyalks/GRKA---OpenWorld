using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSettings
    {
        public GameObject prefab;           // Prefab da IA a ser spawnada
        public List<Transform> spawnPoints; // Pontos de spawn para a IA
        public int maxSpawns;               // Número máximo de spawns dessa IA
        public float spawnCooldown;         // Tempo de cooldown para spawns
        [HideInInspector] public int currentSpawnCount = 0;   // Contador atual de spawns
        [HideInInspector] public float lastSpawnTime = 0;     // Tempo do último spawn
    }

    [Header("Spawn Settings")]
    public List<SpawnSettings> spawnSettingsList;   // Lista das configurações de spawn para cada IA

    [Header("Player Trigger Settings")]
    public Collider playerTrigger;  // Trigger para detectar a presença do jogador

    private bool playerInsideTrigger;  // Indica se o jogador está dentro do trigger

    void Start()
    {
        if (playerTrigger == null)
        {
            Debug.LogError("Player trigger collider not assigned to AISpawnManager.");
        }
    }

    void Update()
    {
        if (playerInsideTrigger)
        {
            foreach (var spawnSettings in spawnSettingsList)
            {
                if (spawnSettings.currentSpawnCount < spawnSettings.maxSpawns &&
                    Time.time - spawnSettings.lastSpawnTime >= spawnSettings.spawnCooldown)
                {
                    SpawnAI(spawnSettings);
                }
            }
        }
    }

    void SpawnAI(SpawnSettings spawnSettings)
    {
        if (spawnSettings.spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned for " + spawnSettings.prefab.name);
            return;
        }

        int spawnIndex = Random.Range(0, spawnSettings.spawnPoints.Count);
        Transform spawnPoint = spawnSettings.spawnPoints[spawnIndex];

        Instantiate(spawnSettings.prefab, spawnPoint.position, spawnPoint.rotation);
        spawnSettings.currentSpawnCount++;
        spawnSettings.lastSpawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
        }
    }
}
