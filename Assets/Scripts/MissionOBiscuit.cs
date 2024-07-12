using System.Collections.Generic;
using UnityEngine;

public class MissionOBiscuit : Mission
{
    [SerializeField] GameObject pfOBiscuit;
    [SerializeField] Transform rootObject;
    [SerializeField] List<GameObject> patrolOrigins;
    [SerializeField] List<GameObject> patrolDestinations;
    int gangMembersLeft = 7;

    protected void Awake()
    {
        base.Awake();
        Name = "obiscuit";
        Description = "The 'ol Biscuit gang has burned down John's place. Go there and whipe them out!";
        DestinationLocations.Add(new Vector2(1601.3f, 3514.559f));

        maxTimeGold = 120;
        maxTimeSilver = 240;
    }

    override public void StartMission()
    {
        if (!IsActive && !HasFinished)
        {
            int countOBiscuits = 0;
            foreach (GameObject patrolOrigin in patrolOrigins)
            {
                Vector3 spawnLocation = new Vector3(patrolOrigin.transform.position.x - 45 + Random.value * 90f, 1000, patrolOrigin.transform.position.z - 45 + Random.value * 90f);
                float y = Terrain.activeTerrain.SampleHeight(spawnLocation);
                spawnLocation = new Vector3(spawnLocation.x, y, spawnLocation.z);
                GameObject newOBiscuit = Instantiate(pfOBiscuit, spawnLocation, Quaternion.identity);
                newOBiscuit.name = "OBiscuit" + (countOBiscuits+1);
                newOBiscuit.transform.parent = rootObject;
                // this is a bit cumbersome, but if you create a patrol OriginDestination list object instead of 2 separate lists, it is not displayed in editor
                newOBiscuit.GetComponent<NPC>().SetPatrolDestinations(new Vector2(patrolDestinations[countOBiscuits].transform.position.x, patrolDestinations[countOBiscuits].transform.position.z));
                countOBiscuits++;
            }
            base.StartMission();
        }
    }

    public void DecreaseGangMembers()
    {
        gangMembersLeft--;
        if (gangMembersLeft == 0)
        {
            CompleteMission();
        }
        else
        {
            Game.Instance.DisplayMission("Gang members left: " + gangMembersLeft);

        }
    }

    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
