using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour
{
    [SerializeField]
    private WaypointPath waypointPath;

    [SerializeField]
    private float plataformSpeed;

    private int targetWayPointIndex;

    private Transform previousWayPoint;
    private Transform targetWayPoint;

    private float timeToWayPoint;
    private float elapsedTime;

    private bool objetoAcima;

    public float raioDaPlataforma = 1f;

    void Start()
    {
        TargetNextWayPoint();
        objetoAcima = true;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWayPoint; 
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);

        transform.position = Vector3.Lerp(previousWayPoint.position, targetWayPoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(previousWayPoint.rotation, targetWayPoint.rotation, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWayPoint();
        }

      /*  if (objetoAcima == true)
        {
            Debug.Log("choras1");
            RaycastHit hit;
            Vector3 startPoint = transform.position + Vector3.up * raioDaPlataforma * 0.5f;
            float raySpacing = raioDaPlataforma / Mathf.Sqrt(2);
            int numberOfRays = Mathf.CeilToInt(raioDaPlataforma / raySpacing);

            for (int i = 0; i < numberOfRays; i++)
            {
                Vector3 rayOrigin = startPoint + Vector3.right * raySpacing * i;
                if (Physics.Raycast(rayOrigin, Vector3.up, out hit, raioDaPlataforma))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                       Debug.Log("choras2");
                       hit.collider.transform.SetParent(transform);
                    }
                }
            }
        } */
    }

    private void TargetNextWayPoint()
    {
        previousWayPoint = waypointPath.GetWaypoint(targetWayPointIndex);
        targetWayPointIndex = waypointPath.GetNextWayPointIndex(targetWayPointIndex);
        targetWayPoint = waypointPath.GetWaypoint(targetWayPointIndex);

        elapsedTime = 0;

        float distanceToWayPoint = Vector3.Distance(previousWayPoint.position, targetWayPoint.position);
        timeToWayPoint = distanceToWayPoint / plataformSpeed;
    }
    
    
}
