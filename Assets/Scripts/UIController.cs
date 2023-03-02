using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Control Hearts, Keys, & Powerup Icons
public class UIController : MonoBehaviour
{
    public Image[] Hearts;
    public Sprite fullHeart;
    public Sprite brokenHeart;

    //Hearts
    private bool lastHeart = false;
    private bool heartSmall = false;
    private float heartBlinkTimer = 0;
    private float heartBlinkLim = .5f;
    private float heartSizeChange = .05f;
    private float heartSizeStart;
    private Color blinkColor = new Vector4(.85f, .8f, .8f, 1);

    //Transition

    void Start()
    {
        heartSizeStart = Hearts[0].transform.localScale.x;
    }

    void Update()
    {
        if (lastHeart){
            heartBlinkTimer += Time.deltaTime;

            if (heartBlinkTimer > heartBlinkLim){
                if (heartSmall){
                    Hearts[0].transform.localScale = new Vector3(heartSizeStart, heartSizeStart, 1);
                    Hearts[0].color = blinkColor;
                } else {
                    Hearts[0].transform.localScale = new Vector3(heartSizeStart + heartSizeChange, heartSizeStart + heartSizeChange, 1);
                    Hearts[0].color = Color.white;
                }

                heartSmall = !heartSmall;
                heartBlinkTimer = 0;
            }
        }
    }

    public void BreakHeart(int heartNumber){
        Hearts[heartNumber].sprite = brokenHeart;

        if (heartNumber == 1){
            lastHeart = true;
        }
    }

    public void HealHeart(int heartNumber){
        Hearts[heartNumber].sprite = fullHeart;

        lastHeart = false;
        Hearts[0].transform.localScale = new Vector3(heartSizeStart, heartSizeStart, 1);
        Hearts[0].color = Color.white;
    }
}
