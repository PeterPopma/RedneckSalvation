using UnityEngine;

public class MissionBank2 : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Game.Instance.ActiveMission!=null && Game.Instance.ActiveMission.Name == "bank")
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<Player>() != null)
                {
                    if (collider.gameObject.GetComponent<Player>().ObjectInHand!=null && collider.gameObject.GetComponent<Player>().ObjectInHand.name == "safe")
                    {
                        collider.gameObject.GetComponent<Player>().ReleaseSafe();
                        Game.Instance.DisplayMission("Thanks! Could you throw down a dynamite to blow up the safe?");
                    }
                }
            }
        }

    }
}
