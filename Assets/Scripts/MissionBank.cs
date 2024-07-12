using UnityEngine;

public class MissionBank : Mission
{
    protected void Awake()
    {
        base.Awake();
        Name = "bank";
        Description = "Steal the safe from the bank of Raspberry. Then bring it to the saloon at the hill above St Dutch.";
        DestinationLocations.Add(new Vector2(756.1f, 3053.1f));
        DestinationLocations.Add(new Vector2(1087.83f, 3373.9f));

        maxTimeGold = 120;
        maxTimeSilver = 240;
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
