/*
 * Description: Bonk spelll, a simple spell which only confuses
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkSpell : SpellCard
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
        //give em some BURN
        target.GetComponent<Mage>().StatusEffects.Add(new Confused(1));
        target.GetComponent<SpriteRenderer>().color = Color.yellow;

        //always remove the card
        RemoveCard();
    }
}
