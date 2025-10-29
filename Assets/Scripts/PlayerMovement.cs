using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Deck;
using System;
using JetBrains.Annotations;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private int mnPlayerMaxHealth = 40;
    public int mnPlayerHealth = 40;
    public bool UpdateDisplayHealth = false;

    //Flag indicating if health is added or removed
    public bool HealthChange = false;

    //Health Display Values
    public GameObject mcPlayerHealthBar;
    public GameObject mcPlayerHealthNumberDisplay;
    private Animator mcPlayerHealthBarAnimator;

    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX;
    public ParticleSystem ReloadFX;
    public ParticleSystem ReloadCompleteFX;

    public bool mbAttack = false;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 2f;
    public int maxJumps = 2;
    int jumpsRemaining;

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

    private bool mbChamberTransitionComplete = true;

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

    //List of Entrance positions in a chamber this player may arrive at
    Dictionary<int, Vector2> macPlayerEntranceTransitions = new Dictionary<int, Vector2>();

    //Reference to attack generator child
    public AttackGenerator mcAttackGenerator;

    //Flag indicating that reload is being performed 
    private bool mbReloading;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        mcLevelManagerAccess = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        DeckControllerAccess = GameObject.Find("DeckController").GetComponent<DeckController>();

        MapControllerAccess = GameObject.Find("MapController").GetComponent<MapController>();

        HandControllerAccess = GameObject.Find("HandController").GetComponent<HandController>();

        mcPlayerHealthBarAnimator = mcPlayerHealthBar.GetComponent<Animator>();
        mcPlayerHealthBarAnimator.SetFloat("HealthBarSpeed", 0);
        mcPlayerHealthBarAnimator.Play("HealthSlider");

        InitializeExitTransitions();
    }

    // Update is called once per frame
    void Update()
    {

        ChamberExits lePotentialExit = ChamberTransitionCheck();

        //If player meets bounds of Chamber
        if (lePotentialExit != ChamberExits.eeNone && mbChamberTransitionComplete)
        {
            mbChamberTransitionComplete = false;
            //Trigger Chamber transition routine

            var lcNextChamberAttributes = mcLevelManagerAccess.ChangeChamber(GetCheckExitTaken(lePotentialExit));
            StartCoroutine(ChangeChamberCoroutine(lcNextChamberAttributes.Item1, lcNextChamberAttributes.Item2));
        }

        if (UpdateDisplayHealth)
        {
            UpdateDisplayedHealth(HealthChange);
        }

        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();

        if (mbReloading)
        {
            mbMovementProhibited = true;

            mbReloading = DeckControllerAccess.ChargingReloadCard();
            if (!mbReloading)
            {
                ReloadFX.Stop();
                ReloadCompleteFX.Play();
                mbMovementProhibited = false;
            }
        }
        else
        {
            mbMovementProhibited = false;
        }

        if (!isWallJumping && !mbMovementProhibited)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            Flip();
        }


        mbUpStrike = Input.GetKey(KeyCode.W);
        mbDownStrike = Input.GetKey(KeyCode.S);

        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("attackFlip", mbAttackFlip);
        animator.SetBool("Reload", mbReloading);

        animator.SetTrigger("Reset");

    }

    //METHOD:: Receives Damage input to the enemy
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection)
    {
        mnPlayerHealth -= pnDamageAmount;

        UpdateDisplayHealth = true;

        if (mnPlayerHealth <= 0)
        {
            //TODO GAME OVER
        }
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
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void ShiftDeckCounterClockWise(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            DeckControllerAccess.ShiftDeckAction(true);
        }
    }

    public void ShiftDeckClockWise(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DeckControllerAccess.ShiftDeckAction(false);
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
                smokeFX.Play();

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
            MapControllerAccess.MapSwitch();
        }
    }

    //Adds card to hand
    public void AddCardToHand(InputAction.CallbackContext context)
    {
        if (context.performed && !DeckControllerAccess.GetAddHandActivate())
        {
            //DeckControllerAccess.IncrementHand()
            if (DeckControllerAccess.IncrementHand())
            {
                HandControllerAccess.AnimateHand();
            }
        }
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

    public void WallSlide()
    {
        //Not grounded & On a wall & movement != 0
        if (!grounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
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


    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("jump");
                smokeFX.Play();
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpsRemaining--;
                smokeFX.Play();
            }
        }

        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            //Jump away from wall
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;
            animator.SetTrigger("jump");
            smokeFX.Play();

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
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            grounded = true;
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

    private ChamberExits ChamberTransitionCheck()
    {
        ChamberExits leExitTaken = ChamberExits.eeNone;

        if(Physics2D.OverlapBox(ChamberTransitionTopCheckPos.position, ChamberTransitionCheckSize, 0, ChamberTransitionTopLayer))
        {
            leExitTaken = ChamberExits.eeTop;
        }
        else if(Physics2D.OverlapBox(ChamberTransitionBottomCheckPos.position, ChamberTransitionCheckSize, 0, ChamberTransitionBottomLayer))
        {
            leExitTaken = ChamberExits.eeBottom;
        }
        else if (Physics2D.OverlapBox(ChamberTransitionLeftCheckPos.position, ChamberTransitionCheckSize, 0, ChamberTransitionLeftLayer))
        {
            leExitTaken = ChamberExits.eeLeft;
        }
        else if (Physics2D.OverlapBox(ChamberTransitionRightCheckPos.position, ChamberTransitionCheckSize, 0, ChamberTransitionRightLayer))
        {
            leExitTaken = ChamberExits.eeRight;
        }

        return leExitTaken;
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if(rb.velocity.y == 0)
            {
                smokeFX.Play();
            }
        }
    }

    private ChamberExits GetCheckExitTaken(ChamberExits leBaseExit)
    {
        ChamberExits lePlayerExit = leBaseExit;

        if(leBaseExit == ChamberExits.eeLeft && transform.position.y > 4)
        {
            lePlayerExit = ChamberExits.eeMiddleLeft;
        }
        else if(leBaseExit == ChamberExits.eeRight && transform.position.y > 4)
        {
            lePlayerExit = ChamberExits.eeMiddleRight;
        }
        else if(leBaseExit == ChamberExits.eeTop && 
            (meCurrentChamberSize == ChamberSize.eeLarge || meCurrentChamberSize == ChamberSize.eeLong))
        {
            lePlayerExit = (transform.position.x > 0) ? ChamberExits.eeTopRight : ChamberExits.eeTopLeft;
        }
        else if (leBaseExit == ChamberExits.eeBottom &&
            (meCurrentChamberSize == ChamberSize.eeLarge || meCurrentChamberSize == ChamberSize.eeLong))
        {
            lePlayerExit = (transform.position.x > 0) ? ChamberExits.eeBottomRight : ChamberExits.eeBottomLeft;
        }

        return lePlayerExit;
    }

    private Vector2 CheckExitTransition(ChamberSize peNextChamberSize, ChamberExits peNextChamberEntrance)
    {
        Vector2 mcTransitionPosition = new();

        if(macPlayerEntranceTransitions.ContainsKey((((int)peNextChamberSize * 10) + (int)peNextChamberEntrance)))
        {
            mcTransitionPosition = macPlayerEntranceTransitions[(((int)peNextChamberSize * 10) + (int)peNextChamberEntrance)];
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
        float lfWideXOffset = 7f;
        float lfMiddleYOffset = 7f;
        float lfHighYOffset = 14f;
        float lfDefaultRightOffset = 15f;
        float lfRightWideOffset = 22f;

        float lfBaseY = -2.3f;

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
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : 0))));

            //Bottom Left Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeBottomLeft),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfWideXOffset : 0),
                (lfBaseY)));

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
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : 0))));

            //Bottom Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeBottom),
                new Vector2(-11f + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeLong) ? lfDefaultRightOffset : 5),
                (lfBaseY)));

            //Top Entrance
            macPlayerEntranceTransitions.Add((((int)leSizes * 10) + (int)ChamberExits.eeTop),
                new Vector2((-11f + lfWideXOffset),
                (lfBaseY + ((leSizes == ChamberSize.eeLarge || leSizes == ChamberSize.eeTall) ? lfHighYOffset : lfMiddleYOffset))));

        }
    }

    IEnumerator ChangeChamberCoroutine(ChamberSize peNextChamberSize, ChamberExits peNextChamberEntrance)
    {
        rb.velocity = new Vector2(0, 0);

        //Restict Player Movement
        mbMovementProhibited = true;

        //Wait for seconds
        yield return new WaitForSeconds(0.5f);

        //Update player position
        transform.position = CheckExitTransition(peNextChamberSize, peNextChamberEntrance);

        //Wait for seconds
        yield return new WaitForSeconds(0.5f);

        //resume control
        mbMovementProhibited = false;

        meCurrentChamberSize = peNextChamberSize;

        mbChamberTransitionComplete = true;
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

