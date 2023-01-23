using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerType {
        jump,
        walljump,
        key
    };

    public PowerType currentPower;
}
