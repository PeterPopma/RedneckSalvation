using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    Player player;
    ThirdPersonController thirdPersonController;
    Animator animator;
    bool isPlayerRidingHorse;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        thirdPersonController = player.gameObject.GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);

        foreach (var collider in colliders)
        {
            if (!isPlayerRidingHorse && collider.GetComponent<Player>() != null)
            {
                isPlayerRidingHorse = true; 
                //animator.Play("walk", 0, 0);
                transform.parent = player.transform;
                player.Horse = this;
                player.SetRidingHorse(true);
                transform.localPosition = new Vector3(0, -3.0f, 0);
                transform.localRotation = Quaternion.identity;
            }
        }

        if (isPlayerRidingHorse)
        {
            if (thirdPersonController.Speed>0)
            {
                animator.SetLayerWeight(1, 1);
            }
            else
            {
                animator.SetLayerWeight(1, 0);
            }
        }
    }

    public void StopRidingHorse()
    {
        isPlayerRidingHorse = false;
        transform.parent = null;
        transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z + 3);
        animator.SetLayerWeight(1, 0);
    }
}
