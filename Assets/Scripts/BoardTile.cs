/*
 * Description: The control for a board tile, which primarily checks for getting clicked and communicates it to the mage
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    GameManager game;
    public bool moveable = false;
    public bool shootable = false;
    public Vector3 directionOfAttack;

    private void Start()
    {
        game = GameManager.instance;
    }

    public void OnMouseDown()
    {
        if (game.playerWithAction != null)
        {
            if (moveable)
            {
                //tell the character to move and where to
                game.actionPlayerScript.ProcessMove(transform.position);
            }

            if (shootable)
            {
                game.actionPlayerScript.ProcessAttack(transform.position, game.primedSpell);
            }
        }
    }

    public void ResetConditions()
    {
        moveable = false;
        shootable = false;
    }
}
