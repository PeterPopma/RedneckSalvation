using UnityEngine;

public class Explodable : MonoBehaviour
{
    public void Explode()
    {
        if (transform.parent.name == "saloon_hill")
        {
            if (Game.Instance.ActiveMission != null && Game.Instance.ActiveMission.Name == "bank")
            {
                Game.Instance.DisplayMission("Maybe that was not such a good idea.. Thanks anyway, good job!", 3);
                Game.Instance.ActiveMission.CompleteMission(12);
            }
            else
            {   
                // don't blow up the saloon yet!
                return;
            }
        }

        foreach (Transform child in transform.parent)
        {
            Rigidbody newRigidBody = child.gameObject.AddComponent<Rigidbody>();
            newRigidBody.AddForce(new Vector3(Random.value * 8, 10, Random.value * 8), ForceMode.VelocityChange);
            newRigidBody.AddTorque(Random.insideUnitSphere * 5, ForceMode.VelocityChange);
        }
    }
}
