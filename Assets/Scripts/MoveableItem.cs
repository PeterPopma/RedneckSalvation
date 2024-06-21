using UnityEngine;

public class MoveableItem : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject playerHand;
    Rigidbody rigidbody;
    float timeLeftThrowing;

    public float TimeLeftThrowing { get => timeLeftThrowing; set => timeLeftThrowing = value; }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (timeLeftThrowing > 0)
        {
            timeLeftThrowing -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (player.ObjectInHand==null && collider.GetComponent<Player>()!=null && timeLeftThrowing<=0)
        {
            player.PutObjectInHand(gameObject);
            rigidbody.isKinematic = true;
            transform.parent = playerHand.transform;
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;
        }
    }
}