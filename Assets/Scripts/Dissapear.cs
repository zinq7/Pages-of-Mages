/* Description: A script to make a gameObject and it's text child gradually dissapear;
 * Author: Ben Farmilo
 * Date: 2022/08/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dissapear : MonoBehaviour
{
    bool dissapear;
    public Image img;
    public Text txt;
    List<GameObject> resetObjs = new List<GameObject>();
    public float speed;
    public Color prevColor;
    public Color prevChildColor;

    //start
    private void Start()
    {
        txt = gameObject.GetComponentInChildren<Text>();
        img = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dissapear)
        {
            //subtract speed/60 to the opacity
            Color clr = img.color;
            Color clr2 = txt.color;
            clr2.a -= 2 * speed / 60;
            clr.a -= speed / 60;

            //if either is 0, just end it
            if (clr2.a <= 0 || clr.a <= 0)
            {
                dissapear = false;
                txt.gameObject.SetActive(false);
                img.gameObject.SetActive(false);
                clr = prevColor;
                clr2 = prevChildColor;
            }

            txt.color = clr2;
            img.color = clr;
        }
    }

    public void ShowMessage()
    {
        if (txt == null) { Start(); }

        if (resetObjs.Count == 0)
        {
            resetObjs.Add(txt.gameObject);
            resetObjs.Add(img.gameObject);

            img.gameObject.SetActive(true);
            img.gameObject.SetActive(true);
        }
        else
        {
            foreach (GameObject obj in resetObjs)
            {
                obj.SetActive(true);
            }
        }
        
        dissapear = true;
    }
}
