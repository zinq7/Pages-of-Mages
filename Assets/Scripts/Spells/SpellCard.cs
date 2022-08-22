/*
 * Description: The class for every spell card, in order to easily make a deck/hand. NOTE: This is attached to the spell card, not the canvas
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : MonoBehaviour
{
    protected GameManager game;
    public List<GameObject> targets;
    public Debuff debuff;
    public Sprite cardBack;
    public int damage;
    public int knockback;
    public string spellAnim;
    public bool hitAll;
    bool startDying;

    public virtual void Start()
    {
        game = GameManager.instance;
    }

    public virtual void ExtraEffects(GameObject caster, GameObject target)
    {
        //override in subclasses
        //always remove the card
        RemoveCard();
    }

    public virtual void RemoveCard()
    {
        startDying = true;
    }

    public void FixedUpdate()
    {
        if (startDying == true)
        {
            Vector3 pos = gameObject.transform.parent.position;
            pos.x *= 1.03f;
            gameObject.transform.parent.position = pos;

            if (Mathf.Abs(pos.x) > 13)
            {
                game.DrawNewCard(gameObject.transform.parent.gameObject);

                startDying = false;
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
