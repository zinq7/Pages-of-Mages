/*
 * Description: The Spell Fireball (two damage and burn, directional range)
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSpell : SpellCard
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
            targets = casterScript.RangeAttack(2, true);
            game.primedSpell = this;
        }

    }

    public override void ExtraEffects(GameObject caster, GameObject target)
    {
        //give em some BURN
        target.GetComponent<Mage>().StatusEffects.Add(new Burn(2, target));

        //always remove the card
        RemoveCard();
    }
}
