using UnityEngine;

public class MissionGuy : MonoBehaviour
{
    [SerializeField] GameObject mission;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>() != null)
        {
            mission.GetComponent<Mission>().StartMission();
        }
    }
}
