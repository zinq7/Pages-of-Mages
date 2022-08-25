/*
 * Description: Heals target 1 each turn
 * Author: Jack Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regen : Debuff 
{
    public int timeLeft;
    public override void TriggerEffect(Mage mageScript)
    {
        SpriteRenderer sprt = mageScript.gameObject.GetComponent<SpriteRenderer>();
        sprt.color = Color.green;

        mageScript.TakeHit(-1);

        timeLeft--;
     

        if (timeLeft == 0)
        {
            remove = true;
            sprt.color = Color.white;
        }
    }
    
    public Regen(int severity, GameObject mage)
    {
        SpriteRenderer sprt = mage.GetComponent<SpriteRenderer>();
        sprt.color = Color.green;
        timeLeft = severity;
        mage.GetComponent<Mage>().AddEffect(severity * 2);

        fxName = "regen";
    }
}
