using UnityEngine;

public class Swing : MonoBehaviour
{
    float currentAngle;
    bool angleIncreasing;
    [SerializeField] float speed;
    [SerializeField] float maxAngle;

    private void Start()
    {
        currentAngle = -maxAngle + (2 * maxAngle * Random.value);
        angleIncreasing = Random.value < 0.5f;
    }

    void Update()
    {
        if (angleIncreasing)
        {
            currentAngle += speed * Time.deltaTime;
            if (currentAngle > maxAngle)
            {
                angleIncreasing = false;
            }
        }
        else
        {
            currentAngle -= speed * Time.deltaTime;
            if (currentAngle < -maxAngle)
            {
                angleIncreasing = true;
            }
        }
        transform.rotation = Quaternion.Euler(-90 + currentAngle, 0, 0);
    }
}
