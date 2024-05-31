using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnCollisionEnter(Collision collision)
    {
        player.Explode();
    }

    private void OnTriggerEnter(Collider collider)
    {
        player.Explode();
    }
}
