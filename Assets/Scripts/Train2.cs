using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train2 : MonoBehaviour
{
    Player player;
    [SerializeField] Vector3 startPosition;
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
            transform.Rotate(new Vector3(0, 180, 0));
            if (phase > 1)
            {
                phase = 0;
            }
        }

        if (phase == 0)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
        }
        else
        {
            transform.position = Vector3.Lerp(endPosition, startPosition, progress);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        player.Explode();
    }

}
