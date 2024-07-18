using UnityEngine;

public class Swing2 : MonoBehaviour
{
    float currentAngle;
    bool angleIncreasing;
    float currentAngle2;
    bool angleIncreasing2;
    [SerializeField] float speed;
    [SerializeField] float maxAngle;

    private void Start()
    {
        currentAngle = -maxAngle + (2 * maxAngle * Random.value);
        angleIncreasing = Random.value < 0.5f;
        currentAngle2 = -maxAngle + (2 * maxAngle * Random.value);
        angleIncreasing2 = Random.value < 0.5f;
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
        if (angleIncreasing2)
        {
            currentAngle2 += speed * Time.deltaTime;
            if (currentAngle2 > maxAngle)
            {
                angleIncreasing2 = false;
            }
        }
        else
        {
            currentAngle2 -= speed * Time.deltaTime;
            if (currentAngle2 < -maxAngle)
            {
                angleIncreasing2 = true;
            }
        }
        transform.rotation = Quaternion.Euler(currentAngle2, 0, currentAngle);
    }
}
