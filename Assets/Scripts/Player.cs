using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    const int WEAPON_PUNCH = 0, WEAPON_PISTOL = 1, WEAPON_RIFLE = 2, WEAPON_DYNAMITE = 3;
    const int LAYER_RIDE = 1, LAYER_SHOOT = 2, LAYER_PUNCH = 3, LAYER_HOLDRIFLE = 4, LAYER_FIRERIFLE = 5, LAYER_THROW = 6;

    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform vfxFireGun;
    [SerializeField] private GameObject handPosition;
    [SerializeField] private CinemachineCamera vcamAim;
    [SerializeField] private CinemachineCamera vcamHorse;
    [SerializeField] private CinemachineCamera vcamRagdoll;
    [SerializeField] private GameObject bloodSpawnPosition;
    [SerializeField] private GameObject pfBullet;
    [SerializeField] private GameObject gunFirePistol;
    [SerializeField] private GameObject gunFireRifle;
    [SerializeField] private Image aimCursor;
    [SerializeField] private GameObject aimPoint;
    [SerializeField] private GameObject imageIconWeapon;
    [SerializeField] private GameObject imageUnmount;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject dynamite;
    [SerializeField] private GameObject pfDynamite;
    //    [SerializeField] private GameObject hitIndicator;
    //    [SerializeField] private Material matIndicator1;
    //    [SerializeField] private Material matIndicator2;
    [SerializeField] private Rig rigPistol;
    [SerializeField] private Rig rigRifle;
    [SerializeField] private Rig rigHorse;
    [SerializeField] private Transform pfShell;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Transform hips;

    private Horse horse;
    private AudioSource soundGunshot;
    private AudioSource soundWoosh;
    private AudioSource soundPunch;
    private AudioSource soundScreamMale;
    private AudioSource soundScreamMale2;
    bool smokeCreated;
    bool isAiming;
    Animator animator;
    float timeLeftShooting;
    float timeLeftPunching;
    float timeLeftBlood;
    float timeLeftExploded; 
    float timeLastDynamiteThrow;
    private bool throwingDynamite = false;
    private bool thrownDynamite = false;
    private Vector3 hitPosition;
    private RigBuilder rigBuilder;
    private Transform hitTransForm;
    private CharacterController characterController;
    private NPC hitNPC;
    bool hitEnemy;
    int activeWeapon;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public float TimeLeftPunching { get => timeLeftPunching; set => timeLeftPunching = value; }
    public Horse Horse { get => horse; set => horse = value; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gunFirePistol.SetActive(false);
        gunFireRifle.SetActive(false);
        aimCursor.enabled = false;
        rigBuilder = gameObject.GetComponent<RigBuilder>();
        soundGunshot = GameObject.Find("/Sound/Gunshot").GetComponent<AudioSource>();
        soundWoosh = GameObject.Find("/Sound/Woosh").GetComponent<AudioSource>();
        soundPunch = GameObject.Find("/Sound/Punch").GetComponent<AudioSource>();
        soundScreamMale = GameObject.Find("/Sound/ScreamMale").GetComponent<AudioSource>();
        soundScreamMale2 = GameObject.Find("/Sound/ScreamMale2").GetComponent<AudioSource>();
        characterController = gameObject.GetComponent<CharacterController>();
        pistol.SetActive(false);
        rifle.SetActive(false);
        dynamite.SetActive(false);
        rigPistol.weight = 0;
        rigRifle.weight = 0;
        rigHorse.weight = 0;
        imageUnmount.SetActive(false);
        particleSystem.Stop();

        activeWeapon = WEAPON_PISTOL;
        rigPistol.weight = 1;
        rigRifle.weight = 0;
        pistol.SetActive(true);
        imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_pistol");
    }

    public void Hit()
    {
        if (Random.value < .5)
        {
            soundScreamMale.Play();
        }
        else
        {
            soundScreamMale2.Play();
        }
        particleSystem.transform.position = bloodSpawnPosition.transform.position;
        particleSystem.Play();
        timeLeftBlood = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeftExploded > 0)
        {
            timeLeftExploded -= Time.deltaTime;
            if (timeLeftExploded < 0)
            {
                float y = Terrain.activeTerrain.SampleHeight(hips.position);
                if (hips.position.y - y > 1)
                {
                    // not on the ground yet
                    timeLeftExploded = 2f;
                }
                else
                {
                    transform.position = new Vector3(hips.position.x, y + 1, hips.position.z);
                    characterController.enabled = true;
                    animator.enabled = true;
                    vcamRagdoll.enabled = false;
                }
            }
        }
        if (timeLeftBlood > 0)
        {
            timeLeftBlood -= Time.deltaTime;
            if (timeLeftBlood < 0)
            {
                particleSystem.Stop();
            }
        }
        if (Horse != null && rigPistol.weight==0 && rigRifle.weight==0 && timeLeftPunching<=0)
        {
            rigHorse.weight = 1;
        }
        else
        {
            rigHorse.weight = 0;
        }
        if (timeLeftPunching > 0)
        {
            timeLeftPunching -= Time.deltaTime;
            if (timeLeftPunching < 0)
            {
                animator.SetLayerWeight(LAYER_PUNCH, 0);
            }
            if (hitEnemy==false && timeLeftPunching < 0.8f)
            {
                hitEnemy = true;
                float hitDistance = (transform.position - hitPosition).magnitude;
                if (hitDistance < 4f && hitNPC!=null)
                {
                    soundPunch.Play();
                    HandleNPCHit();
                }
            }
        }
        if (timeLeftShooting > 0)
        {
            timeLeftShooting -= Time.deltaTime;
            if (timeLeftShooting < 0)
            {
                animator.SetLayerWeight(LAYER_SHOOT, 0);
                animator.SetLayerWeight(LAYER_HOLDRIFLE, 0);
                animator.SetLayerWeight(LAYER_FIRERIFLE, 0);
            }
            if (timeLeftShooting < 0.6 && !smokeCreated)
            {
                smokeCreated = true;
                Instantiate(vfxFireGun, handPosition.transform.position, Quaternion.identity);
                if (activeWeapon == WEAPON_PISTOL)
                {
                    gunFirePistol.SetActive(true);
                }
                else
                {
                    gunFireRifle.SetActive(true);
                }
            }
            if (timeLeftShooting < 0.5)
            {
                gunFirePistol.SetActive(false);
                gunFireRifle.SetActive(false);
            }
        }
        if (throwingDynamite)
        {
            if (thrownDynamite == false && Time.time >= timeLastDynamiteThrow + 0.7f)
            {
                dynamite.SetActive(false);
                thrownDynamite = true;
                Instantiate(pfDynamite, handPosition.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            }
            if (Time.time >= timeLastDynamiteThrow + 0.7f)
            {
                animator.SetLayerWeight(LAYER_THROW, Mathf.Lerp(animator.GetLayerWeight(LAYER_THROW), 0f, Time.deltaTime * 10f));
            }
            if (Time.time >= timeLastDynamiteThrow + 5.9f)
            {
                dynamite.SetActive(true);
                throwingDynamite = false;
                thrownDynamite = false;
            }
        }

        HandleAiming();
        HandleShooting();
    }

    public void HandleNPCHit()
    {
        hitNPC.Hit(hitPosition, transform.position);
        particleSystem.transform.position = hitPosition;
        particleSystem.Play();
        timeLeftBlood = 0.5f;
    }

    private void HandleShooting()
    {
        //Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            hitTransForm = raycastHit.transform;
            hitPosition = raycastHit.point;
//            hitIndicator.GetComponent<Renderer>().material = matIndicator1;
        }
        else
        {
            // we didn't hit anything, so take a point in the direction of the ray
            hitPosition = ray.GetPoint(10);
            hitTransForm = null;
//            hitIndicator.GetComponent<Renderer>().material = matIndicator2;
        }
//        hitIndicator.transform.position = hitPosition;
        if (hitTransForm != null)
        {
            hitNPC = hitTransForm.GetComponent<NPC>();
        }
        else
        {
            hitNPC = null;
        }
    }

    private void HandleAiming()
    {
        if (isAiming)
        {
            rigBuilder.layers[0].active = true;
            rigBuilder.layers[1].active = true;
            vcamAim.enabled = true;
            aimCursor.enabled = true;

            Vector3 aimLocationXZ = new Vector3(hitPosition.x, hitPosition.y, hitPosition.z);
            aimLocationXZ.y = transform.position.y;

            // Turn player towards aim point (only x and z axis)
            transform.forward = Vector3.Lerp(transform.forward, (aimLocationXZ - transform.position).normalized, Time.deltaTime * 10f);

            // Move the aim target
            aimPoint.transform.position = hitPosition;
        }
        else
        {
            rigBuilder.layers[0].active = false;
            rigBuilder.layers[1].active = false;
            vcamAim.enabled = false;
            aimCursor.enabled = false;
        }
    }

    public void SetRidingHorse(bool isRiding)
    {
        imageUnmount.SetActive(isRiding);
        vcamHorse.enabled = isRiding;
        gameObject.GetComponent<ThirdPersonController>().IsRidingHorse = isRiding;
        if (isRiding)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);  
            gameObject.GetComponent<CharacterController>().center = new Vector3(0, -1.2f, 0);
            gameObject.GetComponent<ThirdPersonController>().MoveSpeed = 12;
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 24;
            animator.SetLayerWeight(LAYER_RIDE, 1);
        }
        else
        {
            Horse.StopRidingHorse();
            Horse = null;
            transform.position = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
            gameObject.GetComponent<CharacterController>().center = new Vector3(0, 1.83f, 0);
            gameObject.GetComponent<ThirdPersonController>().MoveSpeed = 6;
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 12;
            animator.SetLayerWeight(LAYER_RIDE, 0);
        }
    }

    private void OnUnmount()
    {
        if (Horse != null)
        {
            SetRidingHorse(false);
        }
    }

    private void OnShoot()
    {
        if (activeWeapon == WEAPON_PUNCH && timeLeftPunching <= 0)
        {
            timeLeftPunching = 1.2f;
            animator.Play("Punch", LAYER_PUNCH, 0f);
            animator.SetLayerWeight(LAYER_PUNCH, 1f);
            soundWoosh.Play();
            hitEnemy = false;
        }
        if (activeWeapon == WEAPON_PISTOL)
        {
            timeLeftShooting = 0.7f;
            animator.Play("Shoot", LAYER_SHOOT, 0);
            animator.SetLayerWeight(LAYER_SHOOT, 1);
            smokeCreated = false;
            soundGunshot.Play();
            HandleShooting();
            if (hitNPC != null)
            {
                if (hitNPC.TimesHit > 1 && !hitNPC.HasDied && hitNPC.UseRagdoll == false && Random.value > 0.5)
                {
                    hitNPC.NpcState = NPCState_.StandingStill;
                    GameObject bullet = Instantiate(pfBullet, gunFirePistol.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.Setup(hitPosition, this);
                }
                else
                {
                    HandleNPCHit();
                }
            }
        }
        if (activeWeapon == WEAPON_RIFLE)
        {
            timeLeftShooting = 0.7f;
            animator.Play("FireRifle", LAYER_FIRERIFLE, 0);
            animator.SetLayerWeight(LAYER_FIRERIFLE, 1);
            smokeCreated = false;
            soundGunshot.Play();
            HandleShooting();
            Instantiate(pfShell, rifle.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            if (hitNPC != null)
            {
                if (hitNPC.TimesHit > 1 && !hitNPC.HasDied && hitNPC.UseRagdoll == false && Random.value > 0.5)
                {
                    hitNPC.NpcState = NPCState_.StandingStill;
                    GameObject bullet = Instantiate(pfBullet, gunFireRifle.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.Setup(hitPosition, this);
                }
                else
                {
                    HandleNPCHit();
                }
            }
        }
        if (activeWeapon == WEAPON_DYNAMITE)
        {
            timeLastDynamiteThrow = Time.time;
            throwingDynamite = true;
            animator.Play("Throw", LAYER_THROW, 0f);
            animator.SetLayerWeight(LAYER_THROW, 1f);
        }
    }

    public void Explode()
    {
        vcamRagdoll.enabled = true;
        characterController.enabled = false;
        animator.enabled = false;
        rigidbody.AddForce(new Vector3(0, 80, 0), ForceMode.VelocityChange);
        rigidbody.AddTorque(Random.insideUnitSphere, ForceMode.VelocityChange);
        //rigidbodyMain.AddExplosionForce(300, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), 3);
        timeLeftExploded = 4;
    }


    private void OnWeaponSelect()
    {
        activeWeapon++;
        animator.SetLayerWeight(LAYER_SHOOT, 0f);
        animator.SetLayerWeight(LAYER_PUNCH, 0f);
        animator.SetLayerWeight(LAYER_HOLDRIFLE, 0f);
        animator.SetLayerWeight(LAYER_FIRERIFLE, 0f);
        if (activeWeapon > WEAPON_DYNAMITE)
        {
            activeWeapon = WEAPON_PUNCH;
        }
        if (activeWeapon == WEAPON_PUNCH)
        {
            rigPistol.weight = 0;
            rigRifle.weight = 0;
            dynamite.SetActive(false);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_fist");
        }
        if (activeWeapon == WEAPON_PISTOL)
        {
            rigPistol.weight = 1;
            rigRifle.weight = 0;
            pistol.SetActive(true);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_pistol");
        }
        if (activeWeapon == WEAPON_RIFLE)
        {
            rigPistol.weight = 0;
            rigRifle.weight = 1;
            pistol.SetActive(false);
            rifle.SetActive(true);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_rifle");
        }
        if (activeWeapon == WEAPON_DYNAMITE)
        {
            dynamite.SetActive(true);
            rifle.SetActive(false);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_dynamite");
        }
    }

    private void OnAim(InputValue value)
    {
        isAiming = value.isPressed;
        if (activeWeapon == 1)
        {
            animator.Play("Shoot", LAYER_SHOOT, 0);
            animator.SetLayerWeight(LAYER_SHOOT, 1);
        }
        if (activeWeapon == 2)
        {
            animator.Play("HoldRifle", LAYER_HOLDRIFLE, 0);
            animator.SetLayerWeight(LAYER_HOLDRIFLE, 1);
        }
        if (!isAiming)
        {
            animator.SetLayerWeight(LAYER_HOLDRIFLE, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            look = value.Get<Vector2>();
        }
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
