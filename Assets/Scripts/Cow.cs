using UnityEngine;

public class Cow : MonoBehaviour
{
    bool animationStarted;
    Animation animation;
    float animationDelay;

    private void Start()
    {
        animation = GetComponent<Animation>();
        animationDelay = Random.Range(0, 5f);
    }

    void FixedUpdate()
    {
        if (!animationStarted && Time.time > animationDelay)
        {
            animationStarted = true; 
            animation.Play();
        }
    }
}
