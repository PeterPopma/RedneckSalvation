using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    const int LAYER_JUMP = 6;
    [SerializeField] private Transform vfxSmoke;
    [SerializeField] private Transform smokePosition;
    private const float TIME_BETWEEN_SMOKE = 1.5f;
    readonly List<AudioSource> soundGallop = new();
    Player player;
    ThirdPersonController thirdPersonController;
    Animator animator;
    bool isPlayerRidingHorse;
    private int animIDSpeed;
    private int animationLayerPlaying;
    private float timeLeftAnimation;
    private float timeleftGallop;
    private float timeleftDust;
    private int currentSound;
    private bool isJumping;

    public bool IsJumping { get => isJumping; set => isJumping = value; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        animIDSpeed = Animator.StringToHash("Speed");
        soundGallop.Add(GameObject.Find("/Sound/Gallop1").GetComponent<AudioSource>());
        soundGallop.Add(GameObject.Find("/Sound/Gallop2").GetComponent<AudioSource>());
        soundGallop.Add(GameObject.Find("/Sound/Gallop3").GetComponent<AudioSource>());
    }

    public void Jump()
    {
        isJumping = true;
        timeLeftAnimation = 1.2f;
        animationLayerPlaying = LAYER_JUMP;
        animator.SetLayerWeight(animationLayerPlaying, 1);
        animator.Play("Jump", animationLayerPlaying, 0);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        thirdPersonController = player.gameObject.GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);

        foreach (var collider in colliders)
        {
            if (!isPlayerRidingHorse && collider.GetComponent<Player>() != null)
            {
                StartRidingHorse();
            }
        }
    }

    private void PlayRandomAnimation()
    {
        animationLayerPlaying = Random.Range(1, 6);
        animator.SetLayerWeight(animationLayerPlaying, 1);
        animator.Play("Idle" + animationLayerPlaying.ToString(), animationLayerPlaying, 0);
        switch (animationLayerPlaying)
        {
            case 1:
                timeLeftAnimation = 3.09f;
                break;
            case 2:
                timeLeftAnimation = 4.19f;
                break;
            case 3:
                timeLeftAnimation = 4.19f;
                break;
            case 4:
                timeLeftAnimation = 4.19f;
                break;
            case 5:
                timeLeftAnimation = 4.19f;
                break;
        }
    }

    private void GallopSound()
    {
        timeleftGallop -= Time.deltaTime * thirdPersonController.AnimationBlend;
        if (timeleftGallop < 0)
        {
            timeleftGallop = 10;
            soundGallop[currentSound].Play();
            currentSound++;
            if (currentSound >= soundGallop.Count)
            {
                currentSound = 0;
            }
        }
    } 

    private void ShowDust()
    {
        timeleftDust -= Time.deltaTime * thirdPersonController.AnimationBlend;
        if (timeleftDust < 0)
        {
            timeleftDust = TIME_BETWEEN_SMOKE;
            Transform newEffect = Instantiate(vfxSmoke, smokePosition.position, Quaternion.identity);
            newEffect.parent = Game.Instance.EffectsParent.transform;
        }
    }

    public void Update()
    {
        if (isPlayerRidingHorse)
        {
            if(!isJumping)
            {
                GallopSound();
            }
            ShowDust();
            animator.SetFloat(animIDSpeed, thirdPersonController.AnimationBlend);
            // there is movement in this animation, so make sure the horse stays with the player.
            transform.localPosition = new Vector3(0, -2.3f, 0);
        }
        else
        {
            if (animationLayerPlaying == 0)
            {
                // play an animation on average every 2.5 seconds.
                if (Random.value < 0.008f)
                {
                    PlayRandomAnimation();
                }
            }
        }
        if (timeLeftAnimation != 0)
        {
            timeLeftAnimation -= Time.deltaTime;
            if (timeLeftAnimation <= 0)
            {
                timeLeftAnimation = 0;
                animator.SetLayerWeight(animationLayerPlaying, 0);
                animationLayerPlaying = 0;
            }
        }
    }

    private void StartRidingHorse()
    {
        isPlayerRidingHorse = true;
        transform.parent = player.transform;
        player.Horse = this;
        player.SetRidingHorse(true);
        transform.localPosition = new Vector3(0, -2.3f, 0);
        transform.localRotation = Quaternion.identity;
        animator.SetLayerWeight(animationLayerPlaying, 0);
    }

    public void StopRidingHorse()
    {
        isPlayerRidingHorse = false;
        transform.parent = null;
        transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z + 3);
    }
}
