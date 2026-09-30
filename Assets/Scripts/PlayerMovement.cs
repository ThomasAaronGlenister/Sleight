using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Deck;
using System;
using JetBrains.Annotations;
using TMPro;
using TMPro.Examples;

public class PlayerMovement : MonoBehaviour
{
    private int mnPlayerMaxHealth = 40;
    public int mnPlayerHealth = 40;
    public bool UpdateDisplayHealth = false;

    //Amount of Money the player has
    private int mnCoinAmount = 0;

    //Flag indicating if health is added or removed
    public bool HealthChange = false;

    //Health Display Values
    public GameObject mcPlayerHealthBar;
    public GameObject mcPlayerHealthNumberDisplay;
    private Animator mcPlayerHealthBarAnimator;

    //Pause Manager 
    public PauseManager mcPauseManager;

    //Reference to the Money counter UI element
    [SerializeField] private GameObject mcMoneyCountDisplay;

    //Reference to the Text field that shows the attack the hand holds
    [SerializeField] private GameObject mcAttackHandNameDisplay;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem smokeFX;
    [SerializeField] private ParticleSystem mcLandFX;
    [SerializeField] private ParticleSystem ReloadFX;
    [SerializeField] private ParticleSystem ReloadCompleteFX;

    public bool mbAttack = false;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 2f;
    public int maxJumps = 1;
    public int jumpsRemaining;

    [Header("Dashing")]
    public float mfDashSpeed = 10f;
    public float mfDashDuration = 0.3f;
    public float mfDashCooldown = 2f;
    private bool mbIsDashing = false;
    private bool mbCanDash = true;
    TrailRenderer mcDashTrail;

    [Header("Rolling")]
    public float mfRollSpeed = 15f;
    public float mfRollDuration = 0.1f;
    public float mfRollCooldown = 1f;
    private bool mbIsRolling = false;
    private bool mbCanRoll = true;

    [Header("Acceleration")]
    public float acceleration = 2f;

    [Header("ChamberTransitionTopCheck")]
    public Transform ChamberTransitionTopCheckPos;
    public LayerMask ChamberTransitionTopLayer;

    [Header("ChamberTransitionBottomCheck")]
    public Transform ChamberTransitionBottomCheckPos;
    public LayerMask ChamberTransitionBottomLayer;

    [Header("ChamberTransitionRightCheck")]
    public Transform ChamberTransitionRightCheckPos;
    public LayerMask ChamberTransitionRightLayer;

    [Header("ChamberTransitionLeftCheck")]
    public Transform ChamberTransitionLeftCheckPos;
    public LayerMask ChamberTransitionLeftLayer;

    public Vector2 ChamberTransitionCheckSize = new Vector2(0.35f, 1.15f);

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask groundLayer;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool mbAttackFlip = true;

    //Wall Jumping
    float wallJumpDirection;
    float wallJumpTime = 0.2f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    //Reference to Level Manager used to change chambers
    LevelManager mcLevelManagerAccess;

    //Player Placement booleans
    bool isWallJumping;
    bool isFacingRight = true;
    public bool grounded = true;

    public float maxMoveSpeed = 60f;

    private bool mbMovementProhibited = false;

    private ChamberSize meCurrentChamberSize = ChamberSize.eeDefault;

    public bool mbUpStrike = false;
    public bool mbDownStrike = false;
    public bool mbSlowStrike = true;


    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    public DeckController DeckControllerAccess;
    public MapController MapControllerAccess;
    public HandController HandControllerAccess;

    //Player Sprite Renderer
    SpriteRenderer mcSpriteRenderer;

    //Base color
    private Color mcOriginalColor;

    //List of Entrance positions in a chamber this player may arrive at
    Dictionary<int, Vector2> macPlayerEntranceTransitions = new Dictionary<int, Vector2>();

    //Reference to attack generator child
    public AttackGenerator mcAttackGenerator;

    //Flag indicating that reload is being performed 
    private bool mbReloading;

    private bool mbTakingDamage = false;

    //Set of Vectors that define cardinal directions
    public Vector2[] macDirectionVectors = new Vector2[4];

    //The Chamber the player is currently in
    public Chamber mcCurrentChamber;

    private float mfPlayerBoxColliderSizeY;

    //Camera Controller used for various effects
    [SerializeField] private CameraController mcCameraController;

    //Movement Skill Checks
    private bool mbDodgeRollEnabled = false;
    private bool mbDoubleJumpEnabled = false;
    private bool mbDashEnabled = false;
    private bool mbWallClingEnabled = false;

    //Used to rotate the world 
    public int mnWorldMapOrientation = 0;
    public bool mbRotateWorld = false;

    //flag indicating if map is open
    private bool mbMapOpen = false;

    //Flag indicating if the player is in front of a door
    private bool mbDoorOverlap = false;

    [SerializeField] private MapCameraController mcMapCameraController;

    private Transform mcOverlapDoor = null;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.2f;

        animator = GetComponent<Animator>();

        mcLevelManagerAccess = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        DeckControllerAccess = GameObject.Find("DeckController").GetComponent<DeckController>();

        MapControllerAccess = GameObject.Find("MapController").GetComponent<MapController>();

        HandControllerAccess = GameObject.Find("HandController").GetComponent<HandController>();

        mcSpriteRenderer = GetComponent<SpriteRenderer>();

        mcOriginalColor = mcSpriteRenderer.color;

        mcDashTrail = GetComponent<TrailRenderer>();

        mfPlayerBoxColliderSizeY = GetComponent<CapsuleCollider2D>().size.y;

        mcPlayerHealthBarAnimator = mcPlayerHealthBar.GetComponent<Animator>();
        mcPlayerHealthBarAnimator.SetFloat("HealthBarSpeed", 0);
        mcPlayerHealthBarAnimator.Play("HealthSlider");

        macDirectionVectors[(int)AttackDirection.eeRightward] = Vector2.right;
        macDirectionVectors[(int)AttackDirection.eeLeftward] = Vector2.left;
        macDirectionVectors[(int)AttackDirection.eeUpwards] = Vector2.up;
        macDirectionVectors[(int)AttackDirection.eeDownwards] = Vector2.down;

        InitializeExitTransitions();

        //Set Coin amount UI
        AdjustCoinCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateDisplayHealth)
        {
            UpdateDisplayedHealth(HealthChange);
        }

        GroundCheck();

        if(!mbIsDashing)
        {
            Gravity();
        }
     
        WallSlide();
        WallJump();

        if (mbReloading)
        {
            mbMovementProhibited = true;
            rb.velocity = rb.velocity / 2;

            mbReloading = DeckControllerAccess.ChargingReloadCard();
            if (!mbReloading)
            {
                ReloadFX.Stop();
                ReloadCompleteFX.Play();
                mbMovementProhibited = false;
            }
        }
        else if(!mbTakingDamage)
        {
            mbMovementProhibited = false;
        }

        if (!isWallJumping && !mbMovementProhibited && !mbIsDashing && !mbIsRolling)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            Flip();
        }

        //If controller is connected 
        if(Gamepad.current != null)
        {
            mbUpStrike = Input.GetKey(KeyCode.W) || Gamepad.current.leftStick.ReadValue().y > 0.5f;
            mbDownStrike = (Input.GetKey(KeyCode.S) || Gamepad.current.leftStick.ReadValue().y < -0.5f) && !grounded;
        }
        else
        {
            mbUpStrike = Input.GetKey(KeyCode.W);
            mbDownStrike = (Input.GetKey(KeyCode.S)) && !grounded;
        }

        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("attackFlip", mbAttackFlip);
        animator.SetBool("Reload", mbReloading);
        animator.SetBool("Damage", mbTakingDamage);
        animator.SetBool("Grounded", grounded);

        if (!mbIsDashing && !mbIsRolling)
        {
            animator.SetTrigger("Reset");
        }

    }

    //METHOD:: Heals player health
    public void Heal(int pnHealAmount)
    {

    }

    //Enables the dodge roll
    public void PlayEnterDoor()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("PlayerEnterDoor");
    }

    public void PlayTurn(bool pfClockwiseRotation = false)
    {
        //Flip player sprite depending on current flip and direction of rotation
        if((pfClockwiseRotation && isFacingRight) || (!pfClockwiseRotation && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

        animator.Play("PlayerTurn");
    }

    public void SetPlayerAnimatorScaledTime()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    //METHOD:: Receives Damage input to the player
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection)
    {
        if(!mbTakingDamage)
        {
            StartCoroutine(TakeDamage(pnDamageAmount, pnKnockBack, peAttackDirection));
        }
    }

    /*
     * METHOD: Coroutine to Take Damage function called to update player health and apply knockback
     */
    private IEnumerator TakeDamage( int pnDamage, float pnKnockBack, AttackDirection peAttackDirection)
    {
        mnPlayerHealth -= pnDamage;

        if (mnPlayerHealth <= 0)
        {
            //TODO GAME OVER
        }

        UpdateDisplayHealth = true;

        mbTakingDamage = true;
        mbMovementProhibited = true;

        //Direction is Waypoint minus the current enemy position
        Vector2 lcDirection = macDirectionVectors[(int)peAttackDirection];

        lcDirection.y += 2f;

        lcDirection.x += lcDirection.x;

        rb.velocity = Vector2.zero;

        rb.AddForce(lcDirection * 2, ForceMode2D.Impulse);

        mcSpriteRenderer.color = Color.red;

        mcCameraController.ScreenShake(0.2f, 0.5f, 60);

        //TODO: Set to attacks stun time
        yield return new WaitForSeconds(0.2f);
        mcSpriteRenderer.color = mcOriginalColor;
        mbMovementProhibited = false;

        //yield return new WaitForSeconds(mrInvulnerabilityTime);
        mbTakingDamage = false;
    }

    //Updates UI to Displayed Health
    //pbHealthDirection = false : Remove Health
    //pbHealthDirection = true : Add Health
    public void UpdateDisplayedHealth(bool pbHealthDirection)
    {
        AnimatorStateInfo stateInfo =
            mcPlayerHealthBarAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.speedMultiplier == 0)
        {
            mcPlayerHealthBarAnimator.SetFloat("HealthBarSpeed", ((!pbHealthDirection) ? 1.5f : -1.5f));
        }

        //Update Health display value
        mcPlayerHealthNumberDisplay.GetComponent<TMP_Text>().text = mnPlayerHealth.ToString();

        float lfNormalizedHealthTime = 1 - ((float)mnPlayerHealth / (float)mnPlayerMaxHealth);

        if ((!pbHealthDirection && stateInfo.normalizedTime >= lfNormalizedHealthTime) ||
            (pbHealthDirection && stateInfo.normalizedTime < lfNormalizedHealthTime))
        {
            mcPlayerHealthBarAnimator.SetFloat("HealthBarSpeed", 0);
            UpdateDisplayHealth = false;
        }
    }

    //Function to Move player
    public void Move(InputAction.CallbackContext context)
    {
        if(mbMapOpen)
        {
            mcMapCameraController.ShiftCamera(context.ReadValue<Vector2>());
        }
        else
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
        }
    }

    //Function to Move player
    public void RotateCamera(InputAction.CallbackContext context)
    {
        if (mbMapOpen)
        {
            //Check Dead zones
            Vector2 lcRotateStick = context.ReadValue<Vector2>();
            if(lcRotateStick.y < 0.5 && lcRotateStick.y > -0.5)
            {
                lcRotateStick.y = 0;
            }

            if (lcRotateStick.x < 0.5 && lcRotateStick.x > -0.5)
            {
                lcRotateStick.x = 0;
            }
            mcMapCameraController.RotateCamera(lcRotateStick);
        }
    }

    //Enables the dash
    public void EnableDash(bool pbCanDash)
    {
        mbDashEnabled = pbCanDash;
    }

    //Function to make player dash
    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && mbCanDash && mbDashEnabled)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    //Enables the dodge roll
    public void EnableDodgeRoll(bool pbCanRoll)
    {
        mbDodgeRollEnabled = pbCanRoll;
    }

    //Function to Roll player
    public void Roll(InputAction.CallbackContext context)
    {
        //Only can roll if character is grounded
        if (context.performed && mbCanRoll && grounded && mbDodgeRollEnabled)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    //Pauses the game
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mcPauseManager.SetFreeze();
        }
    }

    //Coroutine to Dodge Roll
    private IEnumerator RollCoroutine()
    {
        mbCanRoll = false;
        mbIsRolling = true;

        animator.SetTrigger("Roll");

        float lfRollDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(lfRollDirection * mfRollSpeed, 0);

        smokeFX.Play();

        yield return new WaitForSeconds(mfRollDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y);

        mbIsRolling = false;

        yield return new WaitForSeconds(mfRollCooldown);
        mbCanRoll = true;
    }

    //Coroutine to Dash
    private IEnumerator DashCoroutine()
    {
        mbCanDash = false;
        mbIsDashing = true;
        mcDashTrail.emitting = true;

        animator.SetTrigger("Dash");

        float lfDashDirection = isFacingRight ? 1f : -1f;
        rb.gravityScale = 0;

        rb.velocity = new Vector2(lfDashDirection * mfDashSpeed, 0);

        yield return new WaitForSeconds(mfDashDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y);

        mbIsDashing = false;
        mcDashTrail.emitting = false;

        yield return new WaitForSeconds(mfDashCooldown);
        mbCanDash = true;
    }

    public void ShiftDeckCounterClockWise(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (mbMapOpen)
            {
                mcMapCameraController.ZoomCamera(false, false);
            }
        }

        if (context.performed)
        {
            if (mbMapOpen)
            {
                mcMapCameraController.ZoomCamera(true, false);
            }
            else
            {
                DeckControllerAccess.ShiftDeckAction(true);
            }
        }
    }

    public void ShiftDeckClockWise(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            if (mbMapOpen)
            {
                mcMapCameraController.ZoomCamera(false, true);
            }
        }

        if (context.performed)
        {
            if (mbMapOpen)
            {
                mcMapCameraController.ZoomCamera(true, true);
            }
            else
            {
                DeckControllerAccess.ShiftDeckAction(false);
            }
        }
    }

    public void DropThroughPlatform(InputAction.CallbackContext context)
    {
        if(context.performed && grounded)
        {
            mcCurrentChamber.PassThroughPlatform();
        }
    }

    public void EnterDoor(InputAction.CallbackContext context)
    {
        if (context.performed && grounded && mbDoorOverlap && mcOverlapDoor != null)
        {
            mcLevelManagerAccess.EnterNewArea(transform, mcCurrentChamber, mcOverlapDoor);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            List<Card> lacAttack = DeckControllerAccess.GetAttackCards();

            if (DeckControllerAccess.ActivateCard(true) && lacAttack.Count != 0)
            {

                if(DeckControllerAccess.CardsInHand())
                {
                    HandControllerAccess.PlayHand();

                    mcAttackHandNameDisplay.GetComponent<TextMeshPro>().text = "";
                }

                AttackDirection leAttackDirection = AttackDirection.eeRightward;
                
                if(mbUpStrike)
                {
                    leAttackDirection = AttackDirection.eeUpwards;
                }
                else if(mbDownStrike)
                {
                    leAttackDirection = AttackDirection.eeDownwards;
                }
                else if (!isFacingRight)
                {
                    leAttackDirection = AttackDirection.eeLeftward;
                }

                //Pass attack card set along with direction to attack generator
                mcAttackGenerator.GenerateAttack(lacAttack, leAttackDirection, mbAttackFlip, isFacingRight);

                mbAttack = true;

                //Do reverse attack animation next attack
                mbAttackFlip = !mbAttackFlip;
            }
            //Pressing attack on the reload card
            else if(lacAttack.Count == 0)
            {
                mbReloading = true;
                animator.SetTrigger("ReloadStart");
                ReloadFX.Play();
            }
        }
        else if (context.canceled)
        {
            DeckControllerAccess.ActivateCard(false);
            mbReloading = false;
            animator.SetBool("Reload", false);
            ReloadFX.Stop();
        }
    }

    public void SetAttackAnimationValue(AttackAnimationType peAttackAnimation)
    {
        animator.SetInteger("AttackAnimationValue", ((int) peAttackAnimation));
        animator.SetTrigger("attack");
    }

    //Function to Allow an active attack to apply a force to the object that initiated it
    public void ApplyAttackMovement()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }

    //Brings up the map
    public void ShowMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mbMapOpen = !mbMapOpen;
            MapControllerAccess.MapSwitch();

            if(mbMapOpen)
            {
                HandControllerAccess.OpenMap();
            }
            else
            {
                HandControllerAccess.CloseMap();
            }
        }
    }

    //Adds card to hand
    public void AddCardToHand(InputAction.CallbackContext context)
    {
        if (context.performed && !DeckControllerAccess.GetAddHandActivate())
        {
            if (DeckControllerAccess.IncrementHand())
            {
                TextMeshPro lcTextMeshPro = mcAttackHandNameDisplay.GetComponent<TextMeshPro>();

                if (lcTextMeshPro)
                {
                    String lcHandText = "";

                    //Get Attack Attributes from Attack Generator
                    AttackAttributes lcHandAttributes = mcAttackGenerator.GetAttackAttributes(DeckControllerAccess.GetAttackCards());

                    int lnAttackDamage = mcAttackGenerator.CalculateAttackDamage(DeckControllerAccess.GetAttackCards());

                    //Animate hand
                    HandControllerAccess.DrawCard(lnAttackDamage);

                    lcHandText += lcHandAttributes.GetAttackName() + "   " + lnAttackDamage;

                    //Get Attack Effect Percentages
                    for (int lnEffectChance = 0; lnEffectChance < (int)SuitEffect.eeSuitEffectEnd; lnEffectChance++)
                    {
                        if (lcHandAttributes.GetEffectChances()[lnEffectChance] != 0)
                        {
                            lcHandText += " <sprite index=" + (lnEffectChance) +  "> " + lcHandAttributes.GetEffectChances()[lnEffectChance] + "%";
                        }
                    }

                    //Set Name of attack to field under hand
                    lcTextMeshPro.text = lcHandText;
                }
            }
        }
    }

    //Gets the hand of the player 
    public List<Card> GetPlayerHand()
    {
        return DeckControllerAccess.GetAttackCards();
    }

    public void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; // fall increasingly faster
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    //Enables the wall slide/jump
    public void EnableWallSlide(bool pbCanWallSlide)
    {
        mbWallClingEnabled = pbCanWallSlide;
    }

    public void WallSlide()
    {
        //Not grounded & On a wall & movement != 0
        if (!grounded & WallCheck() & horizontalMovement != 0 && mbWallClingEnabled)
        {
            isWallSliding = true;
            smokeFX.Play();
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    //Enables the wall slide/jump
    public void EnableDoubleJump(bool pbCanDoubleJump)
    {
        mbDoubleJumpEnabled = pbCanDoubleJump;
        maxJumps = 2;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 || context.canceled)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("jump");
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpsRemaining--;
            }
        }

        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            //Jump away from wall
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;
            animator.SetTrigger("jump");

            //Force Flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                        ls.x *= -1f;
                        transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //Wall Jump = 0.5f -- jump again = 0.6f
        }
}

    private void GroundCheck()
    {
        if(Physics2D.Raycast(transform.position, Vector2.down, (mfPlayerBoxColliderSizeY / 2) + 0.2f, groundLayer))
        {
            jumpsRemaining = maxJumps;
            if(grounded == false)
            {
                grounded = true;
            }
        }
        else
        {
            grounded = false;
        }
    }

    private bool WallCheck()
    {
        return (Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer));
    }

    public void SetDoorOverlap(bool pbAtDoor, Transform pcDoor, bool pbAutoOpen = false)
    {
        mbDoorOverlap = pbAtDoor;
        mcOverlapDoor = pcDoor;

        //Door may force exit depending on trigger
        if (pbAutoOpen)
        {
            mcLevelManagerAccess.EnterNewArea(transform, mcCurrentChamber, mcOverlapDoor);
        }
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void AdjustCoinCount(int pnCoinAdjustment)
    {
        mnCoinAmount += pnCoinAdjustment;

        TextMeshPro lcTextMeshPro = mcMoneyCountDisplay.GetComponent<TextMeshPro>();

        if (lcTextMeshPro)
        {
            lcTextMeshPro.text = mnCoinAmount.ToString() + " $";
        }
    }

    /**
     * Changes the current chamber the player is present in
     */
    public void ChangeChamber(Chamber pcNewChamber)
    {
        mcCurrentChamber = pcNewChamber;
    }

    private Vector2 CheckExitTransition(ChamberSize peNextChamberSize, ChamberExits peNextChamberEntrance)
    {
        Vector2 mcTransitionPosition = new();

        if(macPlayerEntranceTransitions.ContainsKey((((int)peNextChamberSize * 10) + (int)peNextChamberEntrance)))
        {
            mcTransitionPosition = macPlayerEntranceTransitions[(((int)peNextChamberSize * 10) + (int)peNextChamberEntrance)];
            Debug.Log("Moved Player to X: " + mcTransitionPosition.x + " Y: " + mcTransitionPosition.y);
        }
        else
        {
            Debug.Log("Exit Transition not found");
        }

        Debug.Log("Player Entrance x: " + mcTransitionPosition.x + " y: " + mcTransitionPosition.y + " ChamberSize " + peNextChamberSize +
            " Entrance " + peNextChamberEntrance);

        return mcTransitionPosition;
    }

    private void InitializeExitTransitions()
    {
        //Offsets for larger chambers
        float lfWideXOffset = 6f;
        float lfMiddleYOffset = 7f;
        float lfHighYOffset = 14f;
        float lfDefaultRightOffset = 16f;
        float lfRightWideOffset = 22f;

        float lfBaseY = -2.3f;

        float lfTopEntranceYOffsetLow = 6.5f;
        float lfBottomEntranceYOffset = -4f;

        foreach (ChamberSize leSizes in Enum.GetValues(typeof(ChamberSize)))
        {
            //Left Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeLeft), new Vector2(-11f, lfBaseY));

            //Middle Left Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeMiddleLeft), 
                new Vector2(-11f, (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfMiddleYOffset : 0))));

            //Top Left Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeTopLeft),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfWideXOffset : 0), 
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : lfTopEntranceYOffsetLow))));

            //Bottom Left Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeBottomLeft),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfWideXOffset : 0),
                (lfBaseY + lfBottomEntranceYOffset)));

            //Right Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeRight),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfRightWideOffset : lfDefaultRightOffset),
                (lfBaseY)));

            //Middle Right Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeMiddleRight),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfRightWideOffset : lfDefaultRightOffset),
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfMiddleYOffset : 0))));

            //Top Right Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeTopRight),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfDefaultRightOffset : 0),
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : lfTopEntranceYOffsetLow))));

            //Bottom Right Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeBottomRight),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfDefaultRightOffset : 0),
                (lfBaseY + lfBottomEntranceYOffset)));

            //Bottom Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeBottom),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfDefaultRightOffset : 7.5f),
                (lfBaseY + lfBottomEntranceYOffset)));

            //Top Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeTop),
                new Vector2((-11f + 7.5f),
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : lfMiddleYOffset))));

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheckPos.position, wallCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(ChamberTransitionBottomCheckPos.position, ChamberTransitionCheckSize);
    }
}

