using UnityEngine;

public class Windmill : MonoBehaviour
{
    [SerializeField] Transform wings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wings.Rotate(Time.deltaTime * 15, 0, 0);
    }
}
