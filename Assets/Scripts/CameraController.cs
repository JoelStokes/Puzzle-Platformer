using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform topLeftLim;
    public Transform bottomRightLim;
    private Camera cameraComponent;

    //Fall Wipe Effect
    public GameObject FallWipe;
    private float fallWipeYAdjust = -21;
    private bool playerReset = false;
    private float fallWipeSpeed = 35;

    //End Circle Effect
    public GameObject EndCircle;
    public GameObject BlackCover;
    private bool isEnding = false;
    private float lerpSpeed = 2f;
    private float time = 0;
    private float endCircleSpeed = .75f;
    private float startCameraSize;
    private float endCameraSize = 6;
    private Vector3 startPos;
    private Vector3 endPos;
    private float scaleStop = 0.01f; //Triggers scene is covered enough to fully cover black & load new scene
    private bool circleShrinking = false;

    private Transform player;
    private PlayerController playerScript;
    private float aheadDistance = 1f;
    private float cameraSpeed = 2f;
    private float lookAhead;

    private void Awake() {
        GameObject PlayerObj = GameObject.Find("Player");
        player = PlayerObj.GetComponent<Transform>();
        playerScript = PlayerObj.GetComponent<PlayerController>();

        FallWipe.SetActive(false);
        EndCircle.SetActive(false);
        BlackCover.SetActive(false);
    }

    private void Start(){
        cameraComponent = GetComponent<Camera>();
        startCameraSize = cameraComponent.orthographicSize;
    }

    void Update()
    {
        if (FallWipe.activeSelf){
            FallWipe.transform.position = new Vector3(transform.position.x, FallWipe.transform.position.y + (fallWipeSpeed * Time.deltaTime), FallWipe.transform.position.z);

            if (!playerReset && FallWipe.transform.position.y >= 0){
                playerScript.ResetFromFall();
                playerReset = true;
            }

            if (FallWipe.transform.position.y > transform.position.y + 25){
                FallWipe.transform.position = new Vector3(transform.position.x, transform.position.y + fallWipeYAdjust, FallWipe.transform.position.z);
                FallWipe.SetActive(false);
            }
        }

        if (!isEnding){            
            float newX, newY;

            if (player.position.x > topLeftLim.position.x && player.position.x < bottomRightLim.position.x){
                lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);

                newX = player.position.x + lookAhead;
            } else if (player.position.x <= topLeftLim.position.x) {
                newX = topLeftLim.position.x + lookAhead;
            } else {
                newX = bottomRightLim.position.x + lookAhead;
            }

            if (player.position.y < topLeftLim.position.y && player.position.y > bottomRightLim.position.y){
                newY = player.position.y;
            } else if (player.position.y >= topLeftLim.position.y) {
                newY = topLeftLim.position.y;
            } else{
                newY = bottomRightLim.position.y;
            }

            transform.position = new Vector3(newX, newY, transform.position.z);
        } else {
            Debug.Log("This is happening");

            time += Time.deltaTime/lerpSpeed;
            Vector2 newPos = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, time));
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            cameraComponent.orthographicSize = Mathf.Lerp(startCameraSize, endCameraSize, Mathf.SmoothStep(0.0f, 1.0f, time));

            if (circleShrinking){
                float subtract = endCircleSpeed * Time.deltaTime;
                EndCircle.transform.localScale = new Vector3(EndCircle.transform.localScale.x - subtract, EndCircle.transform.localScale.y - subtract, EndCircle.transform.localScale.z);

                if (EndCircle.transform.localScale.x <= scaleStop){
                    BlackCover.SetActive(true);
                }
            }
        }
    }

    public void StartFallWipe(){
        FallWipe.SetActive(true);
        playerReset = false;
    }

    public void LevelEnding(float endX){
        startPos = transform.position;
        endPos = new Vector3(endX, player.position.y, transform.position.z);
        isEnding = true;
    }

    public void ShrinkCircle(){
        circleShrinking = true;
        EndCircle.SetActive(true);
    }
}
