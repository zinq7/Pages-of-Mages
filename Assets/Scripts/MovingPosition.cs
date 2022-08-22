/*
 * Description: Class to hold direction and rotation as a float
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPosition
{
    public Vector3 position;
    public float direction;
    // Start is called before the first frame update
   public MovingPosition(Vector3 a, float b)
    {
        position = a;
        direction = b;
    }
}
