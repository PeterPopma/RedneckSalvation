using UnityEngine;

public class Tent : MonoBehaviour
{
    int timesHit;
    float offsetX;
    bool increaseX;
    Vector3 startPosition;
    private Player player;

    private void Start()
    {
        startPosition = transform.parent.position;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (timesHit > 0)
        {
            if (increaseX)
            {
                offsetX += timesHit * 1000 * Time.deltaTime;
                if (offsetX > timesHit * 5)
                {
                    increaseX = false;
                }
            }
            else
            {
                offsetX -= timesHit * 1000 * Time.deltaTime;
                if (offsetX < -timesHit * 5)
                {
                    increaseX = true;
                }
            }
            transform.parent.position = new Vector3(startPosition.x + offsetX * .01f, startPosition.y, startPosition.z);
        }
    }

    public void Hit()
    {
        timesHit++;
        if(timesHit > 5)
        {
            player.Explode(600);
            timesHit = 0;
        }
    }
}
