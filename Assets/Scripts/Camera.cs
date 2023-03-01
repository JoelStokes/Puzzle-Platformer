using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform topLeftLim;
    public Transform bottomRightLim;

    private Transform player;
    private float aheadDistance = 1f;
    private float cameraSpeed = 2f;
    private float lookAhead;

    private void Awake() {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
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
}
