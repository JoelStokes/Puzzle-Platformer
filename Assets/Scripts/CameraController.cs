using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform topLeftLim;
    public Transform bottomRightLim;

    //Fall Wipe Effect
    public GameObject FallWipe;
    private Vector3 fallWipeStartPos;
    private bool playerReset = false;
    private float fallWipeSpeed = 30;

    private Transform player;
    private PlayerController playerScript;
    private float aheadDistance = 1f;
    private float cameraSpeed = 2f;
    private float lookAhead;

    private void Awake() {
        GameObject PlayerObj = GameObject.Find("Player");
        player = PlayerObj.GetComponent<Transform>();
        playerScript = PlayerObj.GetComponent<PlayerController>();

        fallWipeStartPos = FallWipe.transform.position;
        FallWipe.SetActive(false);
    }

    void Update()
    {
        if (FallWipe.activeSelf){
            FallWipe.transform.position = new Vector3(FallWipe.transform.position.x, FallWipe.transform.position.y + (fallWipeSpeed * Time.deltaTime), FallWipe.transform.position.z);

            if (!playerReset && FallWipe.transform.position.y >= 0){
                playerScript.ResetFromFall();
                playerReset = true;
            }

            if (FallWipe.transform.position.y > transform.position.y + 25){
                FallWipe.transform.position = fallWipeStartPos;
                FallWipe.SetActive(false);
            }
        }
        Vector3 newPos = transform.position;

        if (player.position.x > topLeftLim.position.x && player.position.x < bottomRightLim.position.x){
            lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);

            newPos = new Vector3(player.position.x + lookAhead, newPos.y, newPos.z);
        } 

        if (player.position.y < topLeftLim.position.y && player.position.y > bottomRightLim.position.y){
            newPos = new Vector3(newPos.x, player.position.y, newPos.z);
        }

        transform.position = newPos;
    }

    public void StartFallWipe(){
        FallWipe.SetActive(true);
        playerReset = false;
    }
}
