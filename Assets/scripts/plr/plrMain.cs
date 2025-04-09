using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class plrMain : MonoBehaviour
{
    // -------------------------------------------------------- VARIABLES --------------------------------------------------------

    [Header("animations")]
	public int pose = 0;
    private int climbPose = 0;
    public bool poseChangeble = true;

    [Header("movement")]
    public float speed = 6.0f, runSpeed = 10.0f, jumpForce = 10.0f;
    public float climbSpeed = 0.05f, climbRunSpeed = 0.08f;
    public bool canMove = true;
    public bool climbing = false;
    public bool groundTouch = false;

    [Header("params")]
    public float health = 100;
	public float stamina = 100;
    public float freezed = 0;
    public float money = 0;
    public int level = 0;
    public bool inWarmZone = false;
	public bool canHealing = true;

    [Header("interact")]
    public GameObject inter, ladder;
    public bool canInteract = true;
    public int doorZone;

    [Header("components")]
    private Rigidbody2D rb2d;
    private Animator animator;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    // --------------- UPDATE ---------------
    void statesVariableUpdate()
    {

        if (health <= 0) { Time.timeScale = 0; _Save.ResetConv(); _Save.GameReset(); SceneManager.LoadScene("Menu"); }

        // Stamina
        if ((Input.GetKey(KeyCode.LeftShift) && rb2d.velocity.x != 0) || climbing)
        {
            if (stamina > 0) stamina -= 5f;
        }
        else if (stamina < 100) stamina += 2.5f;

        // Freeze system
        if (!inWarmZone && freezed < 100) freezed += 1f;
        else if (inWarmZone && freezed > 0) freezed -= 2f;
        else if (!inWarmZone && freezed >= 100 && health > 0) health -= 2f;
        else if (freezed < 50 && inWarmZone && canHealing && health < 100) health += 1f;
    }

    // -------------------------------------------------------- MOVEMENT --------------------------------------------------------

    // --------------- DEFAULT ---------------
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        InvokeRepeating("statesVariableUpdate", 0, 1f);
    }

    private void FixedUpdate()
    {
        // Interact
        if (Input.GetKey(KeyCode.Return)) Invoke("interact", 0);

        // Movement
        float moveInput = Input.GetAxis("Horizontal"), moveInputY = Input.GetAxis("Vertical");
        float currentSpeed = (Input.GetKey(KeyCode.LeftShift) && stamina > 0) ? runSpeed * health/100 : speed * health / 100;
        float climbingSpeed = (Input.GetKey(KeyCode.LeftShift) && stamina > 0) ? climbRunSpeed * health / 100 : climbSpeed * health / 100;

        if (canMove)
        {
            if (!climbing) rb2d.velocity = new Vector2(moveInput * currentSpeed, rb2d.velocity.y);

            if (Input.GetKey(KeyCode.Space) && stamina >= 25 && IsGrounded())
            {
                stamina -= 25;
                if (climbing) { ladder = null; climbing = false; }
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            }

            if (climbing && ladder != null && stamina > 10)
            {
                Handleclimbing(moveInputY, climbingSpeed);
            }
            else
            {
                climbPose = 0;
                rb2d.isKinematic = false;
                climbing = false;
                ladder = null;
            }
        }
        else
        {
            moveInput = 0;
        }

        // Animation pose
        if (poseChangeble)
        {
            UpdateAnimationPose(currentSpeed, moveInput);
        }

        // Animation
        if (!climbing)
        {
            if (moveInput > 0) GetComponent<SpriteRenderer>().flipX = true;
            else if (moveInput < 0) GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    // --------------- COLLISION ---------------
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance);
        return hit.collider != null && groundTouch;
    }

    // -------------------------------------------------------- ANIMATION --------------------------------------------------------
    void UpdateAnimationPose(float currentSpeed, float moveInput)
    {
        Debug.Log(currentSpeed);
        Debug.Log(moveInput);
        Debug.Log(IsGrounded());

        if (!IsGrounded())
        {
            pose = climbPose switch
            {
                0 => rb2d.velocity.y > 0 ? 4 : 3,
                1 => 5,
                -1 => -1,
                _ => -climbPose,
            };
        }
        else if (currentSpeed <= speed && moveInput != 0)
        {
            pose = 1;
        }
        else if (currentSpeed <= runSpeed && moveInput != 0)
        {
            pose = 2;
        }
        else
        {
            pose = 0;
        }
    }

    // -------------------------------------------------------- INTERACTION --------------------------------------------------------
    public void interact()
	{
		if (canInteract && inter) HandleInteractableInteraction();
    }

    void Handleclimbing(float moveInputY, float climbingSpeed)
    {
        BoxCollider2D collider = ladder.GetComponent<BoxCollider2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;

        if (ladder.CompareTag("ladder"))
        {
            rb2d.velocity = Vector2.zero;

            float nextStep = moveInputY * climbingSpeed;
            float nextPos = transform.position.y + nextStep;

            float downSide = ladder.transform.position.y - (collider.size.y / 2) + collider.offset.y;
            float topSide = downSide + collider.size.y;
            Debug.Log(nextStep);

            if (nextPos > downSide && nextPos < topSide) { transform.position = new Vector2(ladder.transform.position.x, transform.position.y + nextStep); climbPose = moveInputY != 0 ? 1 : -1; }
        else
        {
            climbPose = -1;
            if ((transform.position.y - topSide) > 0 && (transform.position.y - topSide) < 5) transform.position = new Vector2(ladder.transform.position.x, topSide - 0.1f); // if it is higher than the ladder and not far, then we place it above the ladder
            else if ((downSide - transform.position.y) > 0 && (downSide - transform.position.y) < 5) transform.position = new Vector2(ladder.transform.position.x, downSide + 0.1f); // if it’s below the ladder and not far, then we put it under the ladder

            else if (
                (transform.position.y - topSide) > 0 && (transform.position.y - topSide) > 5 // if further than 5 pixels from top
                || // or
                (downSide - transform.position.y) > 0 && (downSide - transform.position.y) > 5 // if further than 5 pixels from down
                ) // then reset climbing
            { rb2d.bodyType = RigidbodyType2D.Dynamic; climbing = false; ladder = null; }
        }

            // new /\
            // -----------
            // old \/ 

            //if (transform.position.y + moveInputY * climbingSpeed > ladder.transform.position.y - collider.size.y / 2 + collider.offset.y &&
            //    transform.position.y + moveInputY * climbingSpeed < ladder.transform.position.y + collider.size.y / 2 + collider.offset.y)
            //{
                
            //    transform.position = new Vector2(transform.position.x, transform.position.y + moveInputY * climbingSpeed);
            //    climbPose = moveInputY != 0 ? 1 : -1;
            //}
            //else { climbPose = -1; rb2d.velocity = Vector2.zero; }

            //transform.position = new Vector2(ladder.transform.position.x, transform.position.y);
        }
        else if (ladder.CompareTag("grable"))
        {
            rb2d.velocity = Vector2.zero;
            transform.position = new Vector2(ladder.transform.position.x, ladder.transform.position.y);

            if (_Variables.HasVriablesCommponent(ladder))
            {
                int pose = Mathf.FloorToInt(_Variables.GetFloatVariable(ladder, "pose"));
                climbPose = pose + 3;
            }
        }
    }

    void HandleInteractableInteraction()
    {
        if (inter.CompareTag("ladder") || inter.CompareTag("grable")) { ladder = inter; climbing = true; }
        else if (_Variables.HasVriablesCommponent(inter))
        {
            _Variables.SetVariable(inter, "aktive", true);
            _Variables.SetVariable(inter, "plr", gameObject);
        }
    }
    
    // -------------------------------------------------------- COLLISIONS --------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("inter") || collision.CompareTag("ladder") || collision.CompareTag("grable")) inter = collision.gameObject;
		else if (collision.CompareTag("warmZone")) inWarmZone = true;
        else if (collision.CompareTag("touchInter")) { inter = collision.gameObject; interact(); }

        if(collision.CompareTag("doorZone"))
        {
            doorZone = Mathf.FloorToInt(_Variables.GetFloatVariable(collision.gameObject, "num"));
            Debug.Log(doorZone);
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("inter") || collision.CompareTag("ladder") || collision.CompareTag("grable") || collision.CompareTag("touchInter") ) inter = null;
        else if (collision.CompareTag("warmZone")) inWarmZone = false;

        if (collision.CompareTag("doorZone"))
        {
            doorZone = -1;
            Debug.Log(doorZone);
        }
    }

	void OnCollisionEnter2D(Collision2D collision) { if (collision.collider.gameObject.layer == 6) groundTouch = true; }
    void OnCollisionExit2D(Collision2D collision) { if (collision.collider.gameObject.layer == 6) groundTouch = false; }
}