using System.Collections.Generic;
using UnityEngine;

public class MissionFarm : Mission
{
    protected override void Awake()
    {
        base.Awake();
        Name = "farm";
        Description = "Blow up the old farm, so it can't be used by bandits.";
        DestinationLocations.Add(new Vector2(1792.6f, 2429.5f));

        maxTimeGold = 100;
        maxTimeSilver = 170;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
