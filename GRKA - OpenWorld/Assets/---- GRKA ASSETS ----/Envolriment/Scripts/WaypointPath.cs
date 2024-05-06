using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
   public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWayPointIndex(int currentWayPointIndex)
    {
        int nextWayPointIndex = currentWayPointIndex + 1;

        if(nextWayPointIndex == transform.childCount)
        {
            nextWayPointIndex = 0;
        }

        return nextWayPointIndex;
    }


}
