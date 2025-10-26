using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    public Transform mcPlayerTransform;
    public float mfMovementSpeed = 2f;
    public float mfJumpForce = 2f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask mcGroundLayer;

    private Rigidbody2D mcEnemyRigidBody;
    public bool mbIsGrounded;
    private bool mbShouldJump;

    // Start is called before the first frame update
    void Start()
    {
        mcEnemyRigidBody = GetComponent<Rigidbody2D>();
        StartCoroutine(EnemyActionCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //is grounded? 
        //GroundCheck();

        mbIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, mcGroundLayer);

        //Player Direction
        float lbPlayerDirection = Mathf.Sign(mcPlayerTransform.position.x - transform.position.x);

        //Player above direction
        bool lbPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 << mcPlayerTransform.gameObject.layer);

        if (mbIsGrounded)
        {
            mcEnemyRigidBody.velocity = new Vector2(lbPlayerDirection * mfMovementSpeed, mcEnemyRigidBody.velocity.y);

            RaycastHit2D lcGroundInFront = Physics2D.Raycast(transform.position, new Vector2(lbPlayerDirection, 0), 2f, mcGroundLayer);

            RaycastHit2D lcGapAhead = Physics2D.Raycast(transform.position + new Vector3(lbPlayerDirection, 0, 0), Vector2.down, 2f, mcGroundLayer);

            RaycastHit2D lcPlatformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, mcGroundLayer);

            if (!lcGroundInFront && !lcGapAhead.collider)
            {
                mbShouldJump = true;
            }
            else if (lbPlayerAbove && lcPlatformAbove.collider)
            {
                mbShouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
    }

    IEnumerator EnemyActionCoroutine()
    {
        if (mbIsGrounded && mbShouldJump)
        {
            mbShouldJump = false;

            //Move towards the player
            Vector2 lcMovementDirection = (mcPlayerTransform.position - transform.position).normalized;


            Vector2 lcJumpDirection = lcMovementDirection * mfJumpForce;

            mcEnemyRigidBody.AddForce(new Vector2(lcJumpDirection.x, mfJumpForce), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(2);
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, mcGroundLayer))
        {
            mbIsGrounded = true;
        }
        else
        {
            mbIsGrounded = false;
        }
    }
}

