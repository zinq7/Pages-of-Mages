/*
 * Description: Frozen! Completely loses the enemy one turn
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen : Debuff 
{
    public int timeLeft;
    public override void TriggerEffect(Mage mageScript)
    {
        SpriteRenderer sprt = mageScript.gameObject.GetComponent<SpriteRenderer>();
        sprt.color = Color.blue;

        mageScript.frozen = true;

        timeLeft--;
        
        if (timeLeft == 0)
        {
            remove = true;
            sprt.color = Color.white;
        }
    }

    public Frozen(GameObject target)
    {
        fxName = "frozen";
        timeLeft = 1;
        target.transform.Find("WeaponAnim").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
        target.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1f, 1f);
        GameManager.instance.PlayAnimation(target.GetComponent<Mage>().wpAnim, "freeze");
    }
}
