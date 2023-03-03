using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform topLeftLim;
    public Transform bottomRightLim;

    //Fall Wipe Effect
    public GameObject FallWipe;
    private float fallWipeYAdjust = -21;
    private bool playerReset = false;
    private float fallWipeSpeed = 35;

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
    }

    public void StartFallWipe(){
        FallWipe.SetActive(true);
        playerReset = false;
    }
}
