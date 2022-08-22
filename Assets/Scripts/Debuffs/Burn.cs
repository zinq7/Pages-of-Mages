/*
 * Description: The first "damage only" debuff, cuases damage but also has a visual effect using Flame GameObjects
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Debuff 
{
    public int timeLeft;
    const int Flame_FX = 0;
    public override void TriggerEffect(Mage mageScript)
    {
        SpriteRenderer sprt = mageScript.gameObject.GetComponent<SpriteRenderer>();
        sprt.color = Color.red;

        mageScript.TakeHit(1);

        timeLeft--;
        //delete two flame effects
        for (int i = 0; i < 2; i++)
        {
            GameObject flame = mageScript.gameObject.transform.Find("Flame(Clone)").gameObject;
            flame.SetActive(false);
            flame.transform.SetParent(null, false);
            Object.Destroy(flame);
        }

        if (timeLeft == 0)
        {
            remove = true;
            sprt.color = Color.white;
        }
    }

    public Burn(int severity, GameObject mage)
    {
        SpriteRenderer sprt = mage.GetComponent<SpriteRenderer>();
        sprt.color = Color.red;
        timeLeft = severity;
        mage.GetComponent<Mage>().AddEffect(severity * 2, Flame_FX);

        fxName = "burn";
    }
}
