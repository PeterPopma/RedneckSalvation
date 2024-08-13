using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] pfSpawnObjects;
    [SerializeField] GameObject centerObject;
    [SerializeField] Transform rootTransform;
    [SerializeField] float secondsBetweenSpawns = 1;
    [SerializeField] int initialItems = 0;
    [SerializeField] int maxItems = 100;
    [SerializeField] float minX = -1500;
    [SerializeField] float maxX = 1500;
    [SerializeField] float minZ = -1500;
    [SerializeField] float maxZ = 1500;
    [SerializeField] float centerRadius;

    float timeLastSpawn;


    // Start is called before the first frame update
    void Start()
    {
        timeLastSpawn = Time.time;
        for (int i = 0; i < initialItems; i++)
        {
            SpawnNewObject();
        }
    }

    private void SpawnNewObject()
    {
        int objectIndex = Random.Range(0, pfSpawnObjects.Length);
        int triesLeft = 10;

        Vector3 spawnPosition;
        do
        {
            Vector3 spawnLocation;
            float x, y=0, z;
            triesLeft--;
            if (centerObject == null)
            {
                x = Random.value * (maxX - minX) + minX;
                z = Random.value * (maxZ - minZ) + minZ;
            }
            else
            {
                x = centerObject.transform.position.x - centerRadius + Random.value * centerRadius * 2;
                z = centerObject.transform.position.z - centerRadius + Random.value * centerRadius * 2;
            }
            spawnLocation = new Vector3(x, 0, z);
            // y = terrain.SampleHeight(spawnPosition);
            float terrainHeight;
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                terrainHeight = terrain.SampleHeight(spawnLocation);
                if (y < terrainHeight)
                {
                    y = terrainHeight;
                }
            }
            spawnPosition = new Vector3(x,y,z);
        } while (!NoOtherObjectsNearby(spawnPosition) && triesLeft > 0);
        GameObject newObject = Instantiate(pfSpawnObjects[objectIndex], spawnPosition,
                                                Quaternion.identity);

        newObject.transform.parent = rootTransform;
        Game.Instance.NPCs.Add(newObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (secondsBetweenSpawns > 0 && 
            Time.time - timeLastSpawn > secondsBetweenSpawns && 
            Game.Instance.NPCs.Count < maxItems)
        {
            SpawnNewObject();
            timeLastSpawn = Time.time;
        }
    }

    private bool NoOtherObjectsNearby(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 10);
        foreach (var collider in colliders)
        {
            if (!collider.gameObject.name.StartsWith("Ground"))
            {
                return false;
            }
        }

        return true;
    }
}
