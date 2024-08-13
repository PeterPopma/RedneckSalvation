using UnityEngine;

public class Windmill : MonoBehaviour
{
    [SerializeField] Transform wings;

    void Update()
    {
        wings.Rotate(Time.deltaTime * 15, 0, 0);
    }
}
