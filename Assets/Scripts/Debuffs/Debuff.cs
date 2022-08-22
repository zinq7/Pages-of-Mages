/*
 * Description: Debuf class with virtuala TriggerEffect for putting all debuffs in a list
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff
{
    public string fxName;
    public int DoT;
    public bool remove;

    public virtual void TriggerEffect(Mage mageScript)
    {

    }

}
