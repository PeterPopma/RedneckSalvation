using Unity.Cinemachine;
using UnityEngine;

public class DynamiteLarge : MonoBehaviour
{
    [SerializeField] private Transform vfxExplosion;
    [SerializeField] private GameObject colliderObject;
    private AudioSource soundExplosion;

    void Start()
    {
        soundExplosion = GameObject.Find("/Sound/Explosion").GetComponent<AudioSource>();
    }

    public void Hit()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 20.0f);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Explodable>() != null)
            {
                if (collider.gameObject.transform.name == "ExplosionTriggerFarm")
                {
                    if (Game.Instance.ActiveMission != null && Game.Instance.ActiveMission.Name == "farm")
                    {
                        Game.Instance.ActiveMission.CompleteMission(6);
                    }
                    else
                    {
                        // don't blow up the farm yet!
                        return;
                    }
                }
                collider.gameObject.GetComponent<Explodable>().Explode();
            }
        }
        Destroy(colliderObject);

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
