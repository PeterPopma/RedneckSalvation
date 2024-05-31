using UnityEngine;

public class Shark : MonoBehaviour
{
    Vector3 center;
    float currentAngle;
    const int RADIUS = 20;

    private void Start()
    {
        center = transform.position;
    }

    void Update()
    {
        currentAngle += Time.deltaTime * 1;
        float offset = currentAngle % 2;
        if (offset > 1)
        {
            offset = 2 - offset;
        }
        transform.position = new Vector3(center.x + RADIUS * Mathf.Cos(currentAngle), center.y + offset * 3, center.z + RADIUS * Mathf.Sin(currentAngle));
        transform.rotation = Quaternion.Euler((float)((0.5 - offset) * 20), - currentAngle * 180 / Mathf.PI, 0);
    }
}
