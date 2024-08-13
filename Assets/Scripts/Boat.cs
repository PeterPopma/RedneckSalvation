using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] private List<Vector2> wayPoints = new();
    Quaternion previousRotation;
    Vector2 destination;
    float wobbleX;
    int currentWayPointIndex;
    const float MOVE_SPEED = 0.5f;
    float timeCount = 0.0f;
    Vector3 direction;

    void Start()
    {
        previousRotation = transform.rotation;
        destination = wayPoints[0];
    }

    void FixedUpdate()
    {
        wobbleX += 0.1f;
        if (wobbleX > 10)
        {
            wobbleX = 0;
        }

        Vector2 distanceToDestination = destination - new Vector2(transform.position.x, transform.position.z);
        if (distanceToDestination.sqrMagnitude > 0.25f)
        {
            distanceToDestination.Normalize();
            direction = new Vector3(distanceToDestination.x, 0, distanceToDestination.y);
            transform.rotation = Quaternion.Lerp(previousRotation, Quaternion.LookRotation(direction, Vector3.up), timeCount);
            timeCount += 0.01f;
            if (wobbleX < 5)
            {
                transform.Rotate(new Vector3(wobbleX - 2.5f, 90, 0));
            }
            else
            {
                transform.Rotate(new Vector3(7.5f - wobbleX, 90, 0));
            }
            transform.position += direction * MOVE_SPEED;
        }
        else
        {
            NextWayPoint();
        }
    }

    private void NextWayPoint()
    {
        previousRotation = Quaternion.LookRotation(direction, Vector3.up);
        timeCount = 0;
        currentWayPointIndex++;
        if (currentWayPointIndex >= wayPoints.Count)
        {
            currentWayPointIndex = 0;
        }
        destination = wayPoints[currentWayPointIndex];
    }
}
