using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOverworld : MonoBehaviour
{
    private float deadZone = .25f;
    private bool moving = false;

    private float moveX;
    private float moveY;

    private Vector3[,] routePositions;
    private bool routesSet = false;

    //Bezier Curve Follower
    private float tParam = 0;
    private float speed = 1.5f;

    private Vector2 objectPosition;

    private float speedModifier;

    void Update()
    {
        if (!moving){
            if (moveX > deadZone && routePositions[1,0] != Vector3.zero){  //East
                StartCoroutine(GoByTheRoute(1));
            } else if (moveX < -deadZone && routePositions[3,0] != Vector3.zero){  //West
                StartCoroutine(GoByTheRoute(3));
            } else if (moveY > deadZone && routePositions[0,0] != Vector3.zero){  //North
                StartCoroutine(GoByTheRoute(0));
            } else if (moveY < -deadZone && routePositions[2,0] != Vector3.zero){  //South
                StartCoroutine(GoByTheRoute(2));
            }
        }
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        moving = true;

        Vector2 p0 = routePositions[routeNum,0];
        Vector2 p1 = routePositions[routeNum,1];
        Vector2 p2 = routePositions[routeNum,2];
        Vector2 p3 = routePositions[routeNum,3];

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speed;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;
        moving = false;

    }

    public void MoveX(InputAction.CallbackContext context){
        moveX = context.ReadValue<float>();
    }

    public void MoveY(InputAction.CallbackContext context){
        moveY = context.ReadValue<float>();
    }

    public void Confirm(InputAction.CallbackContext context){
        if (!moving){
            //Start Level
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Level"){        
            routePositions = other.GetComponent<LevelTile>().GetPositions();
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (!routesSet){
            if (other.gameObject.tag == "Level"){        
                routePositions = other.GetComponent<LevelTile>().GetPositions();
                routesSet = true;
            }
        }
    }
}
