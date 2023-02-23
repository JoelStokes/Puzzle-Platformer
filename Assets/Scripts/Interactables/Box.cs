using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public float speed = .15f;

    private bool lerping = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private float time = 0;

    private Rigidbody2D rigi;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayerMask;
    private LayerMask pushLayerMask;    //Avoids detecting One-Ways when pushing horizontally
    private Vector2 boxSize;    //Slightly smaller for checks to help drop down 1 block wide gaps
    private Vector2 boxStartingSize;
    private float sizeSubtractY = 0.1f;
    private float sizeSubtractX = 0.1f;
    private float groundedHeight = .1f;
    private float maxVelocity = 20f;

    private bool isGrounded = false;

    void Start(){
        boxCollider = GetComponent<BoxCollider2D>();
        rigi = GetComponent<Rigidbody2D>();
        groundLayerMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("One-Way");
        pushLayerMask = LayerMask.GetMask("Ground");

        boxStartingSize = boxCollider.size;
        boxSize = new Vector2(boxCollider.bounds.size.x - sizeSubtractX, boxCollider.bounds.size.y - sizeSubtractY);
    }

    void FixedUpdate() {
        if (!isGrounded){
            rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, maxVelocity);
        }

        if (!lerping){
            Debug.Log(CheckGrounded());
            if (CheckGrounded()){
                rigi.isKinematic = true;
                if (!isGrounded){   //Round to nearest .5 to prevent non-grid offsets
                    transform.position = new Vector3(Mathf.Round(transform.position.x*2)/2, Mathf.Round(transform.position.y*2)/2, transform.position.z);
                }

                boxCollider.size = boxStartingSize;
                isGrounded = true;
            } else {
                isGrounded = false;
                rigi.isKinematic = false;

                boxCollider.size = boxSize;
            }
        }
    }

    void Update()
    {
        if (lerping){
            time += (Time.deltaTime * speed);
            transform.position = Vector2.Lerp(startPos, endPos, time);

            if (transform.position.x == endPos.x){
                lerping = false;
                time = 0;
            }
        }
    }

    public void Push(bool pushedLeft){  //Check if next space is vacant, then start lerp in direction
        if (isGrounded && !lerping){
            RaycastHit2D raycastHit;
            if (pushedLeft){
                raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.left, groundedHeight, pushLayerMask);
            } else {
                raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.right, groundedHeight, pushLayerMask);
            }
            if (raycastHit.collider == null){
                startPos = transform.position;
                if (pushedLeft){
                    endPos = new Vector3(startPos.x - 1, startPos.y, startPos.z);
                } else {
                    endPos = new Vector3(startPos.x + 1, startPos.y, startPos.z);
                }

                lerping = true;
            }
        }
    }

    private bool CheckGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.down, groundedHeight, groundLayerMask);
        return raycastHit.collider != null;
    }
}
