using UnityEngine;

public class Fountain : MonoBehaviour
{
    [SerializeField] GameObject water;
    [SerializeField] DayTime dayTime;

    // Update is called once per frame
    void FixedUpdate()
    {
        water.SetActive(dayTime.Elevation > 20);
    }
}
