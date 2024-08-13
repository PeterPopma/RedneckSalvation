using UnityEngine;

public class MissionBank : Mission
{
    protected override void Awake()
    {
        base.Awake();
        Name = "bank";
        Description = "Steal the safe from the bank of Raspberry. Then bring it to the saloon at the hill above St Dutch.";
        DestinationLocations.Add(new Vector2(756.1f, 3053.1f));
        DestinationLocations.Add(new Vector2(1087.83f, 3373.9f));

        maxTimeGold = 120;
        maxTimeSilver = 240;
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

        if (Game.Instance.ActiveMission != null && Game.Instance.ActiveMission.Name == "bank")
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<Player>() != null)
                {
                    if (collider.gameObject.GetComponent<Player>().ObjectInHand != null && collider.gameObject.GetComponent<Player>().ObjectInHand.name == "safe")
                    {
                        collider.gameObject.GetComponent<Player>().ReleaseSafe();
                        Game.Instance.DisplayMission("Thanks! Could you throw down a dynamite to blow up the safe?");
                    }
                }
            }
        }
    }

}
