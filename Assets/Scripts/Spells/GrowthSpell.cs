/*
 * Description: The Spell Growth (Regen to self)
 * Author: Jack Farmilo
 * Date: 2022/08/25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthSpell : SpellCard
{

    // On Spell cast
    public override void CastSpell()
    {
        //always clear the tiles before doing an action
        game.Clear();

        Mage casterScript = game.actionPlayerScript;

        if (casterScript.state == "free")
        {
            targets = casterScript.RangeAttack(0, false, false, Color.green);
            game.primedSpell = this;
        }
    }

    public override void ExtraEffects(GameObject caster, GameObject target)
    {
        //heal up
        caster.GetComponent<Mage>().StatusEffects.Add(new Regen(2, caster));

        //with no animation, it has to end the turn
        game.ActionComplete();

        //always remove the card
        RemoveCard();
    }
}
