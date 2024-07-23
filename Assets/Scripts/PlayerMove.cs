using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Jump System")]
    [SerializeField] private float jumpTime;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    float jumpCounter;
    float horizontalInput;

    public bool isJumping;
    public bool canMove = true;
    private bool isOnSlope;
    public bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;

    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D cc;

    private Vector2 newVelocity;
    private Vector2 colliderSize;
    private Vector2 vecGravity;
    private Vector2 slopeNormalPerp;

    private void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    private void Awake()
    {
        //Grabbing ref
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();

        colliderSize = cc.size;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }
    
    private void FixedUpdate()
    {
        SlopeCheck();
        CheckGround();
    }

    private void Update()
    {
        if (canMove)
        {
            

            horizontalInput = Input.GetAxis("Horizontal");

            if(isGrounded && !isOnSlope && !isJumping )
            {
                newVelocity.Set(speed * horizontalInput, 0.0f);
                body.velocity = newVelocity;
            }
            else if(isGrounded && isOnSlope && !isJumping)
            {
                newVelocity.Set(speed * slopeNormalPerp.x * -horizontalInput, speed * slopeNormalPerp.y * -horizontalInput);
                body.velocity = newVelocity;
            }
            else if (!isGrounded)
            {
                newVelocity.Set(horizontalInput * speed, body.velocity.y);
                body.velocity = newVelocity;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
            
            //Set animator parameters
            anim.SetBool("grounded", isGrounded);
            anim.SetBool("run", horizontalInput != 0);
        }
        else
        {
            canMove = false;
            newVelocity.Set(0.0f, body.velocity.y);
            body.velocity = newVelocity;
            anim.SetBool("grounded", true);
            anim.SetBool("run", false);
        }

        //flip player
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(3, 3, 3);
        if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-3, 3, 3);

        if (body.velocity.y > 0 && isJumping)
            {
                anim.SetTrigger("isJumping");

                jumpCounter += Time.deltaTime;
                if (jumpCounter > jumpTime) isJumping = false;

                body.velocity += vecGravity * jumpMultiplier * Time.deltaTime;
            }

        if (body.velocity.y < 0)
        {
            body.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }
        
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if(isOnSlope && horizontalInput == 0.0f)
        {
            body.sharedMaterial = fullFriction;
        }
        else
        {
            body.sharedMaterial = noFriction;
        }
    }

    private void Jump()
    {

        body.velocity = new Vector2(body.velocity.x, jumpPower);
        isJumping = true;
        jumpCounter = 0;
        
    }

    //private bool isGrounded()
    //{
    //    RaycastHit2D raycastHit = Physics2D.BoxCast(cc.bounds.center, cc.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    //    return raycastHit.collider != null;
    //}
}
