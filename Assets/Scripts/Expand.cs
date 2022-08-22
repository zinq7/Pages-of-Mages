using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expand : MonoBehaviour
{
    Transform trans;
    int expand = 240;
    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (expand <= 120 && expand > 0)
        {
            trans.localScale /= 1.005f;
            expand--;
            trans.Rotate(0, 0, -0.2f);
        }
        else if (expand <= 240 && expand > 0)
        {
            trans.localScale *= 1.005f;
            expand--;
            trans.Rotate(0, 0, -0.2f);
        } 
        else
        {
            expand = 240;
        }
          
    }
}
