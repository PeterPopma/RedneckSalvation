using System.Collections.Generic;
using UnityEngine;

public class MissionLumber : Mission
{
    protected override void Awake()
    {
        base.Awake();
        Name = "lumber";
        Description = "Go to the lumbercamp to cut some trees.";
        DestinationLocations.Add(new Vector2(1214.11f, 1950.25f));

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
