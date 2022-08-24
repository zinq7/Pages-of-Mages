/*
 * Description: Slash! (one damage, one knockback)
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSpell : SpellCard
{
    //edit damage and knockback values in the unity editor

    // On Spell cast
    public override void CastSpell()
    {
        //always clear the tiles before doing an action
        game.Clear();

        Mage casterScript = game.actionPlayerScript;

        if (casterScript.state == "free")
        {
            targets = casterScript.RangeAttack(2, false);
            game.primedSpell = this;
        }
    }
}
