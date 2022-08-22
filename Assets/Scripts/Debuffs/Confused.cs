/*
 * Description: The first effect debuff, takes the player and moves it in a random direction like they would be able to do.
 * Because it uses the opponents turn, there should be a downside to every spell that confuses
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confused : Debuff 
{
    public int timeLeft;
    public override void TriggerEffect(Mage mageScript)
    {
        SpriteRenderer sprt = mageScript.gameObject.GetComponent<SpriteRenderer>();
        sprt.color = Color.yellow;

        mageScript.ClearTilesFree();

        mageScript.state = "confused";

        //create a list of all moveable tiles
        List<GameObject> randomPositions = new List<GameObject>();
        foreach (GameObject tile in GameManager.instance.boardGenerator.activeTiles)
        {
            if (tile.GetComponent<BoardTile>().moveable == true)
            {
                randomPositions.Add(tile);
            }
        }
        //teleport the player to one of them by simulating mouseDown
        randomPositions[Random.Range(0, randomPositions.Count - 1)].GetComponent<BoardTile>().OnMouseDown();


        timeLeft--;
        
        if (timeLeft == 0)
        {
            remove = true;
            sprt.color = Color.white;
        }
    }

    public Confused(int severity)
    {
        fxName = "confused";
        timeLeft = severity;
    }
}
