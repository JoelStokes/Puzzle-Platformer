using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform topLeftLim;
    public Transform bottomRightLim;

    private GameObject player;

    // Start is called before the first frame update
    private void Awake() {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;

        if (player.transform.position.x > topLeftLim.position.x && player.transform.position.x < bottomRightLim.position.x){
            newPos = new Vector3(player.transform.position.x, newPos.y, newPos.z);
        } 

        if (player.transform.position.y < topLeftLim.position.y && player.transform.position.y > bottomRightLim.position.y){
            newPos = new Vector3(newPos.x, player.transform.position.y, newPos.z);
        }

        transform.position = newPos;
    }
}
