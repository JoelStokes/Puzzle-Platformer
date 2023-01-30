using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOverworld : MonoBehaviour
{
    private float deadZone = .6f;
    private float moveSpeed = 5;

    private float moveX;
    private float moveY;

    //Movement Options set by current level tile
    private Vector2 left;
    private Vector2 right;
    private Vector2 up;
    private Vector2 down;

    void Start()
    {
        
    }

    void Update()
    {
        //What's the best way to check moves to next / previous levels?
    }

    public void MoveX(InputAction.CallbackContext context){
        moveX = context.ReadValue<float>();
    }

    public void MoveY(InputAction.CallbackContext context){
        moveY = context.ReadValue<float>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        
    }
}
