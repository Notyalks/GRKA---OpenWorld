using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mission : MonoBehaviour
{
    public string Title { get; private set; }
    public string Objective { get; private set; }
    public bool IsCompleted { get; private set; }

    public event Action OnMissionCompleted;

    public Mission(string title, string objective)
    {
        Title = title;
        Objective = objective;
        IsCompleted = false;
    }

    public void CompleteMission()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnMissionCompleted?.Invoke();
        }
    }
}
