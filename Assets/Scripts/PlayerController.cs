using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Main Player Controller. Setup for low default movement & jumps, with powerups that boost base skills & abilities
public class PlayerController : MonoBehaviour
{
    //Move
    private float accelerationSpeed = .6f;
    private float maxSpeed = 8.5f;
    private float currentMove = 0;
    private float deadZone = .6f;
    private float dampingGround = .8f;
    private float dampingAir = .9f;
    private float dampingTurn = .7f;
    private float dampingTurnAir = .76f;
    private float maxSpeedDampen = 1.25f;
    private float maxSpeedDampenTurn = 1.3f;
    
    //Walk
    private float walkSpeed;    //Set in start, MaxSpeed / 2
    private bool walking = false;

    //Jump
    private float jumpStartForce = 12f;
    private float jumpReleaseMult = .8f;
    private bool jumpStarting = false;
    private bool jumpEnding = false;
    private float jumpPressedTimer = 0;
    private float jumpPressedCount = .2f;

    //Walljump
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private float wallSlowMax = 5;   //Only applied moving downwards
    private float wallJumpOutwardsForce = 8;
    private float wallJumpUpwardsForce = 12f;
    private float wallJumpTimer = 0;
    private float wallJumpCount = .1f;
    private LayerMask wallJumpLayerMask;
    private float wallSlideLeftTimer = 0;
    private float wallSlideRightTimer = 0;
    private float wallSlideCount = .15f;

    //Ground Checks
    private bool isGrounded = false;
    private LayerMask groundLayerMask;
    private float groundedHeight = .1f;
    private float groundedTimer = 0;
    private float groundedCount =.05f;
    private Vector2 groundPos;

    //Fall
    private float maxVelocity = 20; //Prevent player from moving too quickly

    //Powerups
    private bool hiJump = false;
    private bool wallJump = false;
    private int keys = 0;

    //Components
    private Rigidbody2D rigi;
    private BoxCollider2D boxCollider;

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        groundLayerMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("One-Way");
        wallJumpLayerMask = LayerMask.GetMask("Ground");
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (jumpPressedTimer > 0){     //Jump & Grounded buffer windows to allow jump
            jumpPressedTimer -= Time.deltaTime;
        } else {
            jumpStarting = false;
        }

        if (groundedTimer > 0){
            groundedTimer -= Time.deltaTime;
        } else{
            isGrounded = false;
        }

        if (wallJumpTimer > 0){     //Walljump prevents turning around to reach same wall & mashing to double wall jump
            wallJumpTimer -= Time.deltaTime;
        }

        if (wallSlideLeftTimer > 0){
            wallSlideLeftTimer -= Time.deltaTime;
        }

        if (wallSlideRightTimer > 0){
            wallSlideRightTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        if (CheckGrounded()){   //Set Grounded
            groundedTimer = groundedCount;
            isGrounded = true;
            groundPos = new Vector2(transform.position.x, transform.position.y);
        }

        if (!isGrounded){   //Set Walls
            CheckWalls();
        } else {
            onLeftWall = false;
            onRightWall = false;
        }

        if (wallJumpTimer <= 0){     //Move
            if (currentMove > deadZone){
                ApplyMove(1);
            } else if (currentMove < -deadZone){
                ApplyMove(-1);
            } else {
                SlowMovement();
            }
        }

        if (jumpStarting && isGrounded){    //Jump
            ApplyJump(jumpStartForce);
            jumpStarting = false;
            jumpPressedTimer = 0;
            groundedTimer = 0;
        } else if (jumpStarting && ((onLeftWall || onRightWall) || (wallSlideLeftTimer > 0 || wallSlideRightTimer > 0)) && wallJumpTimer <= 0){
            ApplyWallJump();
            jumpPressedTimer = 0;
        } else if (jumpEnding){
            if (rigi.velocity.y > 0){   //Don't apply jump reduction if apex of jump already hit
                ApplyJump(rigi.velocity.y * jumpReleaseMult);
            }
            
            jumpEnding = false;
        }

        rigi.velocity = Vector2.ClampMagnitude(rigi.velocity, maxVelocity); //Prevent falling too fast, avoid clipping through walls
    }

    private bool CheckGrounded(){
        if (rigi.velocity.y <= 0.1f && rigi.velocity.y > -.1f){  //Prevent rising grounded state through semi-solid platforms
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedHeight, groundLayerMask);
            
            return raycastHit.collider != null;
        } else {
            return false;
        }
    }

    private void CheckWalls(){
        RaycastHit2D leftRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, groundedHeight, wallJumpLayerMask);

        if (leftRaycastHit.collider != null && currentMove < -deadZone){
            onLeftWall = true;
        } else {
            if (onLeftWall){    //Was previously in wall slide, give grace period
                wallSlideLeftTimer = wallSlideCount;
            }
            onLeftWall = false;
        }

        RaycastHit2D rightRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, groundedHeight, wallJumpLayerMask);

        if (rightRaycastHit.collider != null && currentMove > deadZone){
            onRightWall = true;
        } else {
            if (onRightWall){
                wallSlideRightTimer = wallSlideCount;
            }
            onRightWall = false;
        }
    }

    private void ApplyMove(float value){
        float dampenedVelocity = rigi.velocity.x;
        if (isGrounded && ((rigi.velocity.x > 0 && value < 0) || (rigi.velocity.x < 0 && value > 0))){  //Used to only apply to isGrounded turning, but feels better jumping like this
            dampenedVelocity *= dampingTurn;
        } else if ((rigi.velocity.x > 0 && value < 0) || (rigi.velocity.x < 0 && value > 0)){
            dampenedVelocity *= dampingTurnAir;
        }

        float maxCheck;
        if (walking){
            maxCheck = walkSpeed;
        } else {
            maxCheck = maxSpeed;
        }

        float newSpeed = ((value * accelerationSpeed) + dampenedVelocity);

        if (newSpeed > maxCheck){  //Fast move right & hold right, retain some momentum. If fast right & trying to slow, immediately drop to maxSpeed
            if (value > 0){
                 newSpeed = newSpeed - maxSpeedDampen;    //Slow degrade to max speed when matching direction
            } else {
                newSpeed = newSpeed - maxSpeedDampenTurn;   //Sharper degrade due to holding against the speed
            }
        } else if (newSpeed < -maxCheck){
            if (value < 0){
                newSpeed = newSpeed + maxSpeedDampen;
            } else {
                newSpeed = newSpeed + maxSpeedDampenTurn;
            }
        }

        rigi.velocity = new Vector2(newSpeed, rigi.velocity.y);
    }

    private void SlowMovement(){    //Dampen current horizontal movement
        float newSpeed = rigi.velocity.x;

        if (isGrounded){
            newSpeed *= dampingGround;
        } else {
            newSpeed *= dampingAir;
        }
        rigi.velocity = new Vector2(newSpeed, rigi.velocity.y);
    }

    private void ApplyJump(float value){
        rigi.velocity = new Vector2(rigi.velocity.x, value);
    }

    private void ApplyWallJump(){
        if (onLeftWall || wallSlideLeftTimer > 0){
            rigi.velocity = new Vector2(wallJumpOutwardsForce, wallJumpUpwardsForce);
        } else if (onRightWall || wallSlideRightTimer > 0){
            rigi.velocity = new Vector2(-wallJumpOutwardsForce, wallJumpUpwardsForce);
        }

        onLeftWall = false;
        onRightWall = false;
        wallJumpTimer = wallJumpCount;
        wallSlideLeftTimer = 0;
        wallSlideRightTimer = 0;
    }

    private void ApplyWallSlow(){
        if (rigi.velocity.y < -wallSlowMax){
            rigi.velocity = new Vector2(rigi.velocity.x, -wallSlowMax);
        }
    }


    public void Jump(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started){
            jumpStarting = true;
            jumpPressedTimer = jumpPressedCount;
        } else if (context.phase == InputActionPhase.Canceled){
            jumpEnding = true;
        }
    }

    public void Move(InputAction.CallbackContext context){
        currentMove = context.ReadValue<float>();
    }

    public void Walk(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started){
            walking = true;
        } else if (context.phase == InputActionPhase.Canceled){
            walking = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Powerup"){
            //Need dynamic system to quickly set one of many different powerup types
        }
    }
}
