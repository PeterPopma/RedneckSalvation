using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] private GameObject effectsParent;
    [SerializeField] private Transform vfxSmoke;
    [SerializeField] private Transform vfxSmoke2;
    [SerializeField] private Transform smokePosition;
    [SerializeField] private float timeBetweenSmoke = 1.5f;
    float timeLastSmoke;


    void Update()
    {
        if (Time.time - timeLastSmoke > timeBetweenSmoke)
        {
            timeLastSmoke = Time.time;
            Transform newEffect = Instantiate(vfxSmoke, smokePosition.position, Quaternion.identity);
            newEffect.parent = effectsParent.transform;
            if (vfxSmoke2 != null)
            {
                newEffect = Instantiate(vfxSmoke2, smokePosition.position, Quaternion.identity);
                newEffect.parent = effectsParent.transform;
            }
        }
    }
}
