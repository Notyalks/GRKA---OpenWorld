using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private List<Mission> missions = new List<Mission>();
    private Mission currentMission;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMission(Mission mission)
    {
        missions.Add(mission);
    }

    public void StartMission(Mission mission)
    {
        currentMission = mission;
        // Display mission to the player (UI)
        Debug.Log($"Mission started: {mission.Title} - {mission.Objective}");
    }

    public void CompleteMission(Mission mission)
    {
        mission.CompleteMission();
        if (currentMission == mission)
        {
            Debug.Log($"Mission completed: {mission.Title}");
            // Handle mission completion (UI, rewards, etc.)
            currentMission = null;
        }
    }

    public Mission GetCurrentMission()
    {
        return currentMission;
    }
}
