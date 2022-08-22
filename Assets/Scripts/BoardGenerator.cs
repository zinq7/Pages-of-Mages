/*
 * Description: Generates a board full of tiles in any shape with different tile weights. Also stores all tiles in a list.
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardGenerator : MonoBehaviour
{
    public GameObject defaultTile;
    public GameObject forestTile;
    public GameObject mountainTile;
    public List<GameObject> activeTiles = new List<GameObject>();
    public List<SpriteRenderer> activeTileRenderers = new List<SpriteRenderer>();

    /// <summary>
    /// Generate a board using wiehgts of each tiles, as well as shape
    /// </summary>
    /// <param name="boardShape">The shape of the board. Index is the Y coordinate</param>
    /// <param name="defaultWeight">weight of default tiles</param>
    /// <param name="forestWeight">weight of forest tiles (slow 1)</param>
    /// <param name="mntWeight">weight of mountain tiles (full block)</param>
    public void GenerateBoard(List<int> boardShape, int defaultWeight = 24, int forestWeight = 4, int mntWeight = 1)
    {
        List<GameObject> WeightedTiles = new List<GameObject>();

        //add the three specified tiles from the person
        for (int i = 0; i < defaultWeight; i++)
        {
            WeightedTiles.Add(defaultTile);
        }

        for (int i = 0; i < forestWeight; i++)
        {
            WeightedTiles.Add(forestTile);
        }

        for (int i = 0; i < mntWeight; i++)
        {
            WeightedTiles.Add(mountainTile);
        }

        //add a default tile for buffer always, in case the board is generated with no wieghts whatsoever (not recommended)
        WeightedTiles.Add(defaultTile);

        //for each Y in the board
        for (int boardY = 0; boardY < boardShape.Count; boardY++)
        {
            //for each X in the board
            for (int boardX = 0; boardX < boardShape[boardY]; boardX++)
            {
                //find the position on the map that would be equal to this list centered
                Vector2 tilePosition = new Vector2(
                    (boardX - boardShape[boardY] / 2) * 2 - (boardShape[boardY] % 2 - 1), //find the center of the board
                    (boardY - boardShape.Count / 2) * 2);

                //select a random tile from the list filled with the weights
                int tileSelectionIndex = Random.Range(0, WeightedTiles.Count - 1);
                GameObject tileType = WeightedTiles[tileSelectionIndex];

                //create the tile at the position
                GameObject tile = Instantiate(tileType, tilePosition, new Quaternion(0, 0, 0, 0), transform);

                //add it to the list, as well as it's SpriteRenderer for easier access later
                activeTiles.Add(tile);
                activeTileRenderers.Add(tile.GetComponent<SpriteRenderer>());
            }
        }
    }


    public void ResetTiles()
    {
        //reset the color of every sprite
        foreach (SpriteRenderer sprt in activeTileRenderers)
        {
            sprt.color = Color.white;
            //also make em not interactable
            sprt.gameObject.GetComponent<BoardTile>().ResetConditions();
        }
    }
}
