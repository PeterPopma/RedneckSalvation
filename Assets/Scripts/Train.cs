using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnTriggerEnter(Collider collider)
    {
        player.Explode();
    }
}
