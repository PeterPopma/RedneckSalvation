using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    Player player;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 middlePosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] float speed;
    private float progress;
    int phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        progress += speed * Time.deltaTime;
        if (progress > 1)
        {
            progress = 0;
            phase++;
            if (phase > 1)
            {
                phase = 0;
            }
        }

        if (phase == 0)
        {
            transform.position = Vector3.Lerp(startPosition, middlePosition, progress);
        }
        else
        {
            transform.position = Vector3.Lerp(middlePosition, endPosition, progress);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        player.Explode();
    }

}
