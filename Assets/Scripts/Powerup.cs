using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerType {
        hiJump,
        wallJump,
        key
    };

    public PowerType currentPower;
}
