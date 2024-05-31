using Unity.Cinemachine;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;
    [SerializeField] private Transform vfxExplosion;
    private AudioSource soundBounce;
    private AudioSource soundExplosion;
    private Vector3 axisOfRotation;
    private Rigidbody myRigidbody;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        float speed = 20f;
        Vector3 velocity = transform.forward * speed;
        velocity.y = 10f;
        myRigidbody.linearVelocity = velocity;
        //axisOfRotation = Random.onUnitSphere;
        axisOfRotation = new Vector3(1, 0.2f, 0.2f);
        soundExplosion = GameObject.Find("/Sound/Explosion").GetComponent<AudioSource>();
        soundBounce = GameObject.Find("/Sound/Bounce").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axisOfRotation, 1000.0f * Time.smoothDeltaTime);

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0f)
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 20.0f);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<Player>() != null)
                {
                    collider.gameObject.GetComponent<Player>().Explode();
                }
                if (collider.gameObject.GetComponent<NPC>() != null)
                {
                    collider.gameObject.GetComponent<NPC>().Explode();
                }
            }
            soundExplosion.Play(); 
            
            CinemachineCamera vcamPlayer = (CinemachineCamera)CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            if (vcamPlayer.GetComponent<CameraShake>() != null)
            {
                vcamPlayer.GetComponent<CameraShake>().ShakeCamera(10, 12f);
            }
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        soundBounce.Play();
    }

}
