using Unity.Cinemachine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Player player;
    private CinemachineCamera vcamBullet;
    public float lifetime = 3.0f;
    private Vector3 moveDir;
    private Vector3 hitPosition;
    private float moveSpeed = 30f;
    private float previousDistance;
    private bool madeHit;

    public void Setup(Vector3 hitPosition, Player player)
    {
        this.player = player;
        this.hitPosition = hitPosition;
        if (hitPosition != null)
        {
            moveDir = (hitPosition - transform.position).normalized;
            previousDistance = Vector3.Distance(transform.position, hitPosition);
            madeHit = false;
        }
        else
        {
            moveDir = transform.forward;
        }
        vcamBullet.Follow = transform;
        vcamBullet.enabled = true;
    }

    private void Awake()
    {
        vcamBullet = GameObject.Find("/PlayerFollowCameraBullet").GetComponent<CinemachineCamera>();
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if (hitPosition == null)
        {
            return;
        }

        if (!madeHit)
        {
            transform.position += moveSpeed * Time.deltaTime * moveDir;
        }

        float distance = Vector3.Distance(transform.position, hitPosition);

        // if the bullet moved passed the object it hits, it should be removed
        if (!madeHit && distance > previousDistance)
        {
            madeHit = true;
            player.HandleNPCHit();
            // put camera before NPC
            transform.position = hitPosition - 8 * moveDir;
            Invoke("EndOfLife", 1.5f);
        }
        previousDistance = distance;

        if (lifetime < 0)
        {
            EndOfLife();
        }
    }

    private void EndOfLife()
    {
        vcamBullet.enabled = false;
        Destroy(gameObject);
    }
}
