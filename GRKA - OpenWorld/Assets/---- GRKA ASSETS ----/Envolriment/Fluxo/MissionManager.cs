using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private string currentMission;

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

    public void SetMission(string mission)
    {
        currentMission = mission;
        Debug.Log("Nova missão: " + mission);
    }

    public string GetCurrentMission()
    {
        return currentMission;
    }

    public void CompleteMission()
    {
        Debug.Log("Missão concluída: " + currentMission);
        currentMission = null;
    }
}
