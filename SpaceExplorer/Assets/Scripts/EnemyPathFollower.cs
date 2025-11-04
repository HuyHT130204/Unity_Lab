using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollower : MonoBehaviour
{
    public List<Transform> pathPoints; // Assigned by spawner
    private int currentPointIndex = 0;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (gameManager.blockControl)
            return;

        if (pathPoints == null || pathPoints.Count == 0)
            return;

        Transform targetPoint = pathPoints[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            2f * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Count;
        }
    }
}

//  void Update()
//     {
//         if (pathPoints == null || pathPoints.Count == 0)
//             return;

//         if (!isFollowingPath)
//         {
//             // Entry phase: move to first path point
//             transform.position = Vector2.MoveTowards(
//                 transform.position,
//                 pathPoints[0].position,
//                 speed * Time.deltaTime
//             );

//             if (Vector2.Distance(transform.position, pathPoints[0].position) < 0.05f)
//             {
//                 isFollowingPath = true;
//                 currentPointIndex = 1; // Start following from pathPoints[1]
//             }
//         }
//         else
//         {
//             Transform targetPoint = pathPoints[currentPointIndex];

//             transform.position = Vector2.MoveTowards(
//                 transform.position,
//                 targetPoint.position,
//                 speed * Time.deltaTime
//             );

//             if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
//             {
//                 currentPointIndex = (currentPointIndex + 1) % pathPoints.Count;
//             }
//         }
//     }
