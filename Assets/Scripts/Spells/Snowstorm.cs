/*
 * Description: Snowstorm, the first case of using HitAll to hit every opponent and freeze
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowstorm : SpellCard
{
    //edit damage and knockback values in the unity editor

    // On Spell cast
    public void CastSpell()
    {
        //always clear the tiles before doing an action
        game.Clear();

        Mage casterScript = game.actionPlayerScript;

        if (casterScript.state == "free")
        {
            targets = casterScript.RangeAttack(1);
            game.primedSpell = this;
        }
    }

    public override void ExtraEffects(GameObject caster, GameObject target)
    {
        //freeze both of them
        target.GetComponent<Mage>().StatusEffects.Add(new Frozen(target));
        caster.GetComponent<Mage>().StatusEffects.Add(new Frozen(caster));

        //call the action complete here?, because the animation doesnt
        game.ActionComplete();

        //always remove the card
        RemoveCard();
    }
}
