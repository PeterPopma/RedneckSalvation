using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    const int WEAPON_PUNCH = 0, WEAPON_PISTOL = 1, WEAPON_RIFLE = 2, WEAPON_DYNAMITE = 3, WEAPON_AXE = 4;
    const int LAYER_RIDE = 1, LAYER_SHOOT = 2, LAYER_PUNCH = 3, LAYER_HOLDRIFLE = 4, LAYER_FIRERIFLE = 5, LAYER_THROW = 6;

    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform vfxFireGun;
    [SerializeField] private GameObject handPosition;
    [SerializeField] private CinemachineCamera vcamAim;
    [SerializeField] private CinemachineCamera vcamPlayerFollow;
    [SerializeField] private CinemachineCamera vcamRagdoll;
    [SerializeField] private GameObject bloodSpawnPosition;
    [SerializeField] private GameObject pfBullet;
    [SerializeField] private GameObject gunFirePistol;
    [SerializeField] private GameObject gunFireRifle;
    [SerializeField] private Image aimCursor;
    [SerializeField] private GameObject aimPoint;
    [SerializeField] private GameObject imageIconWeapon;
    [SerializeField] private GameObject imageUnmount;
    [SerializeField] private GameObject imageRelease;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject axe;
    [SerializeField] private GameObject dynamite;
    [SerializeField] private GameObject pfAxe;
    [SerializeField] private GameObject pfDynamite;
    [SerializeField] private GameObject panelMap;
    [SerializeField] private GameObject playerSpawnPoint;
    //    [SerializeField] private GameObject hitIndicator;
    //    [SerializeField] private Material matIndicator1;
    //    [SerializeField] private Material matIndicator2;
    [SerializeField] private Rig rigPistol;
    [SerializeField] private Rig rigRifle;
    [SerializeField] private Rig rigHorse;
    [SerializeField] private Transform pfShell;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Transform hips;
    [SerializeField] Slider sliderHealthBar;

    private GameObject objectInHand;
    private UI UIScript;
    private Horse horse;
    private AudioSource soundGunshot;
    private AudioSource soundWoosh;
    private AudioSource soundPunch;
    private AudioSource soundDieMale;
    private AudioSource soundScreamMale;
    private AudioSource soundScreamMale2;
    bool smokeCreated;
    bool isAiming;
    Animator animator;
    float timeLeftShooting;
    float timeLeftPunching;
    float timeLeftBlood;
    float timeLeftExploded; 
    float timeLastWeaponThrow;
    float timeLeftDying;
    private bool throwingWeapon = false;
    private bool thrownWeapon = false;
    private Vector3 hitPosition;
    private RigBuilder rigBuilder;
    private Transform hitTransForm;
    private CharacterController characterController;
    private NPC hitNPC;
    bool hitEnemy;
    int activeWeapon;
    int health = 100;
    private int timesStuck;

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
    public GameObject ObjectInHand { get => objectInHand; set => objectInHand = value; }

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
        soundDieMale = GameObject.Find("/Sound/DyingMale3").GetComponent<AudioSource>();
        soundScreamMale = GameObject.Find("/Sound/ScreamMale2").GetComponent<AudioSource>();
        soundScreamMale2 = GameObject.Find("/Sound/ScreamMale3").GetComponent<AudioSource>();
        characterController = gameObject.GetComponent<CharacterController>();
        axe.SetActive(false);
        pistol.SetActive(false);
        rifle.SetActive(false);
        dynamite.SetActive(false);
        rigPistol.weight = 0;
        rigRifle.weight = 0;
        rigHorse.weight = 0;
        imageUnmount.SetActive(false);
        imageRelease.SetActive(false);
        particleSystem.Stop();
        activeWeapon = WEAPON_PISTOL;
        pistol.SetActive(true);
        imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_pistol");
        UIScript = GameObject.Find("/Scripts/UI").GetComponent<UI>();
        sliderHealthBar.value = 1 - (health / 100f);
    }

    public void Hit()
    {
        if (timeLeftDying > 0)
        {
            return;
        }
        particleSystem.transform.position = bloodSpawnPosition.transform.position;
        particleSystem.Play();
        timeLeftBlood = 0.5f;
        health -= 5;
        sliderHealthBar.value = 1 - (health / 100f);
        if (health <= 0)
        {
            soundDieMale.Play();
            Explode(0);
            timeLeftDying = 4;
        }

        if (Random.value < .5)
        {
            soundScreamMale.Play();
        }
        else
        {
            soundScreamMale2.Play();
        }
    }

    public void PutObjectInHand(GameObject objectInHand)
    {
        this.objectInHand = objectInHand;
        imageRelease.SetActive(true);
        if (objectInHand.name == "safe" && Game.Instance.Missions.Find(o => o.Name == "bank").IsActive)
        {
            Game.Instance.Missions.Find(o => o.Name == "bank").NextDestination();
            Game.Instance.DisplayMission("Good! Now bring the safe to the Saloon on the hill above St Dutch.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeftDying > 0)
        {
            timeLeftDying -= Time.deltaTime;
            if (timeLeftDying <= 0)
            {
                SceneManager.UnloadSceneAsync("MainScene");
                SceneManager.LoadSceneAsync("MenuScene");
                Cursor.visible = true;
            }
        }
        if (timeLeftExploded > 0)
        {
            timeLeftExploded -= Time.deltaTime;
            if (timeLeftExploded < 0)
            {
                float y = Game.Instance.MainTerrain.SampleHeight(hips.position);
                if (timesStuck < 10 && hips.position.y - y > 1)
                {
                    // not on the ground yet
                    timeLeftExploded = 2f;
                    timesStuck++;     // prevent getting stuck when there is something wrong with sampling the terrain height
                }
                else
                {
                    timesStuck = 0;
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
                float hitDistance = (transform.position - hitPosition).sqrMagnitude;
                if (hitDistance < 16f && hitTransForm!=null)
                {
                    soundPunch.Play();
                    CheckHit();
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
                Transform newEffect = Instantiate(vfxFireGun, handPosition.transform.position, Quaternion.identity);
                newEffect.parent = Game.Instance.EffectsParent.transform;
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
        if (throwingWeapon)
        {
            if (thrownWeapon == false && Time.time >= timeLastWeaponThrow + 0.7f)
            {
                thrownWeapon = true;
                if (activeWeapon == WEAPON_AXE)
                {
                    axe.SetActive(false);
                    Instantiate(pfAxe, handPosition.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                }
                else
                {
                    dynamite.SetActive(false);
                    Instantiate(pfDynamite, handPosition.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                }
            }
            if (Time.time >= timeLastWeaponThrow + 0.7f)
            {
                animator.SetLayerWeight(LAYER_THROW, Mathf.Lerp(animator.GetLayerWeight(LAYER_THROW), 0f, Time.deltaTime * 10f));
            }
            if (Time.time >= timeLastWeaponThrow + 1.2f)
            {
                FinishThrowing();
            }
        }

        HandleAiming();
        HandleShooting();
    }

    public void FinishThrowing()
    {
        if (activeWeapon == WEAPON_AXE)
        {
            axe.SetActive(true);
        }
        else
        {
            dynamite.SetActive(true);
        }
        throwingWeapon = false;
        thrownWeapon = false;
    }

    public void FixedUpdate()
    {
        if (transform.position.y < -50)
        {
            transform.position = playerSpawnPoint.transform.position;
        }
    }

    public void IncreaseHealth(int amount)
    {
        health += amount;
        if (health > 100)
        {
            health = 100;
        }
        sliderHealthBar.value = 1 - (health / 100f);
    }

    public void HandleNPCHit()
    {
        if (hitNPC != null)
        {
            hitNPC.Hit(transform.position);
        }
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
    }

    private void CheckHit(GameObject gunFire = null)
    {
        if (hitTransForm != null)
        {
            hitNPC = hitTransForm.GetComponent<NPC>();
            if (hitNPC != null)
            {
                if (hitNPC.TimesHit > 1 && !hitNPC.HasDied && hitNPC.UseRagdoll == false && Random.value > 0.5)
                {
                    hitNPC.NpcState = NPCState_.StandingStill;
                    GameObject bullet = Instantiate(pfBullet, gunFire.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.Setup(hitPosition, this);
                }
                else
                {
                    HandleNPCHit();
                }
                return;
            }
            Target target = hitTransForm.GetComponent<Target>();
            if (target != null)
            {
                target.Hit(hitPosition);
                return;
            }
            Tent tent = hitTransForm.GetComponent<Tent>();
            if (tent != null)
            {
                tent.Hit();
                return;
            }
            DynamiteLarge dynamiteLarge = hitTransForm.GetComponent<DynamiteLarge>();
            if (dynamiteLarge != null)
            {
                dynamiteLarge.Hit();
                return;
            }
        }
        else
        {
            hitNPC = null;
        }
    }

    private void HandleAiming()
    {
        if (isAiming && vcamRagdoll.enabled == false)
        {
            rigBuilder.layers[0].active = true;
            rigBuilder.layers[1].active = true;
            vcamAim.enabled = true;
            aimCursor.enabled = true;

            Vector3 aimLocationXZ = new(hitPosition.x, hitPosition.y, hitPosition.z);
            aimLocationXZ.y = transform.position.y;

            // Turn player towards aim point (only x and z axis)
            transform.forward = Vector3.Lerp(transform.forward, (aimLocationXZ - transform.position).normalized, Time.deltaTime * 10f);

            // Move the aim target
            aimPoint.transform.position = hitPosition;

            if (activeWeapon == WEAPON_PISTOL)
            {
                rigPistol.weight = 1;
            }
            if (activeWeapon == WEAPON_RIFLE)
            {
                rigRifle.weight = 1;
            }
        }
        else
        {
            rigPistol.weight = 0;
            rigRifle.weight = 0; 
            rigBuilder.layers[0].active = false;
            rigBuilder.layers[1].active = false;
            vcamAim.enabled = false;
            aimCursor.enabled = false;
        }
    }

    public void SetRidingHorse(bool isRiding)
    {
        if (isRiding == false && Horse == null)
        {
            // nothing to do..
            return;
        }
        imageUnmount.SetActive(isRiding);
        vcamPlayerFollow.GetComponent<CinemachineThirdPersonFollow>().CameraDistance = isRiding ? 20 : Game.Instance.ViewDistance;
        gameObject.GetComponent<ThirdPersonController>().IsRidingHorse = isRiding;
        if (isRiding)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);  
            gameObject.GetComponent<CharacterController>().center = new Vector3(0, -0.5f, 0);
            gameObject.GetComponent<ThirdPersonController>().MoveSpeed = 16;
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 32;
            animator.SetLayerWeight(LAYER_RIDE, 1);
        }
        else
        {
            Horse.StopRidingHorse();
            Horse = null;
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            gameObject.GetComponent<CharacterController>().center = new Vector3(0, 1.83f, 0);
            gameObject.GetComponent<ThirdPersonController>().MoveSpeed = 8;
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 16;
            animator.SetLayerWeight(LAYER_RIDE, 0);
        }
    }

    private void OnUnmount()
    {
        SetRidingHorse(false);
    }

    private void OnRelease()
    {
        if (objectInHand != null)
        {
            imageRelease.SetActive(false);
            objectInHand.GetComponent<MoveableItem>().TimeLeftThrowing = 2;
            objectInHand.transform.parent = null;
            objectInHand.GetComponent<Rigidbody>().isKinematic = false;
            objectInHand.GetComponent<Rigidbody>().AddForce(new Vector3(Random.value * 8, 10, Random.value * 8), ForceMode.VelocityChange);
            objectInHand.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 5, ForceMode.VelocityChange);
            objectInHand = null;
        }
    }

    public void ReleaseSafe()
    {
        imageRelease.SetActive(false);
        objectInHand.transform.parent = null;
        objectInHand.GetComponent<Rigidbody>().isKinematic = false;
        objectInHand.GetComponent<MoveableItem>().Disabled = true;
        objectInHand = null;
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
            animator.Play("Shoot", LAYER_SHOOT, 0);
            animator.SetLayerWeight(LAYER_SHOOT, 1);
            soundGunshot.Play();
            ShootGun(gunFirePistol);
        }
        if (activeWeapon == WEAPON_RIFLE)
        {
            animator.Play("FireRifle", LAYER_FIRERIFLE, 0);
            animator.SetLayerWeight(LAYER_FIRERIFLE, 1);
            soundGunshot.Play();
            Instantiate(pfShell, rifle.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            ShootGun(gunFireRifle);
        }
        if (activeWeapon == WEAPON_DYNAMITE || activeWeapon == WEAPON_AXE)
        {
            timeLastWeaponThrow = Time.time;
            throwingWeapon = true;
            animator.Play("Throw", LAYER_THROW, 0f);
            animator.SetLayerWeight(LAYER_THROW, 1f);
        }
    }

    private void ShootGun(GameObject gunFire)
    {
        timeLeftShooting = 0.7f;
        smokeCreated = false;
        HandleShooting();
        CheckHit(gunFire);
    }

    public void Explode(int strength = 80)
    {
        SetRidingHorse(false);
        vcamRagdoll.enabled = true;
        characterController.enabled = false;
        animator.enabled = false;
        rigidbody.AddForce(new Vector3(0, strength, 0), ForceMode.VelocityChange);
        rigidbody.AddTorque(Random.insideUnitSphere, ForceMode.VelocityChange);
        timeLeftExploded = 4;
    }


    private void OnWeaponSelect()
    {
        activeWeapon++;
        animator.SetLayerWeight(LAYER_SHOOT, 0f);
        animator.SetLayerWeight(LAYER_PUNCH, 0f);
        animator.SetLayerWeight(LAYER_HOLDRIFLE, 0f);
        animator.SetLayerWeight(LAYER_FIRERIFLE, 0f);
        if (activeWeapon > WEAPON_AXE)
        {
            activeWeapon = WEAPON_PUNCH;
        }
        if (activeWeapon == WEAPON_PUNCH)
        {
            axe.SetActive(false);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_fist");
        }
        if (activeWeapon == WEAPON_PISTOL)
        {
            pistol.SetActive(true);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_pistol");
        }
        if (activeWeapon == WEAPON_RIFLE)
        {
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
        if (activeWeapon == WEAPON_AXE)
        {
            dynamite.SetActive(false);
            axe.SetActive(true);
            imageIconWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_axe");
        }
    }

    private void OnAim(InputValue value)
    {
        isAiming = value.isPressed;
        if (activeWeapon == WEAPON_PISTOL)
        {
            animator.Play("Shoot", LAYER_SHOOT, 0);
            animator.SetLayerWeight(LAYER_SHOOT, 1);
        }
        if (activeWeapon == WEAPON_RIFLE)
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

    public void OnToggleMiniMap()
    {
        Game.Instance.ShowMiniMap = !Game.Instance.ShowMiniMap;
        Game.Instance.ShowMessage("Minimap: " + (Game.Instance.ShowMiniMap ? "On" : "Off"));
    }

    public void OnChangeView()
    {
        if (Game.Instance.ViewDistance == 10)
        {
            Game.Instance.ViewDistance = 5;
        }
        else if(Game.Instance.ViewDistance == 5)
        {
            Game.Instance.ViewDistance = 1;
        }
        else
        {
            Game.Instance.ViewDistance = 10;
        }
        vcamPlayerFollow.GetComponent<CinemachineThirdPersonFollow>().CameraDistance = Horse!=null ? 20 : Game.Instance.ViewDistance;
        Game.Instance.ShowMessage("View Distance: " + Game.Instance.ViewDistance);
    }

    public void OnToggleSuperSpeed()
    {
        if (gameObject.GetComponent<ThirdPersonController>().SprintSpeed < 300)
        {
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 300;
        }
        else
        {
            gameObject.GetComponent<ThirdPersonController>().SprintSpeed = 16;
        }
        Game.Instance.ShowMessage("Sprint Speed: " + gameObject.GetComponent<ThirdPersonController>().SprintSpeed);
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
        if (Horse != null)
        {
            Horse.Jump();
        }
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

    private void OnMap(InputValue value)
    {
        UIScript.ShowMap = value.isPressed;
    }

    private void OnProgressScreen(InputValue value)
    {
        UIScript.ToggleAchievementsScreen();
    }

    private void OnHelp()
    {
        UIScript.ToggleHelpScreen();
    }
}
