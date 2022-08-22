/*
 * Description: One of the biggest scripts in the game, it controls all of the actions doable by the player, such as moving and 
 * attacking, as well as stores stats and effects.
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class Mage : MonoBehaviour
{
    //to avoid typing GameManager.instance, the gameManager will be always refered to as "game"
    GameManager game;
    public Animator anim;

    public bool blueTeam, dead;
    public bool frozen;
    public int speed;
    public int health;

    public string state = "free";

    public Sprite blueSprt;
    public Sprite redSprt;
    public Sprite whiteSprt;

    public int move;
    private int realMove;

    private Text hpText;
    private Text spdText;
    public Animator wpAnim;

    public List<Debuff> StatusEffects = new List<Debuff>();
    public List<GameObject> fx = new List<GameObject>();

    public void Start()
    {
        game = GameManager.instance;
        anim = GetComponent<Animator>();

        anim.enabled = false;
        realMove = move;

        //0 is the aura, se thte color to the team
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = blueTeam ? blueSprt : redSprt;

        //set the speed text to the move
        spdText = gameObject.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
        spdText.text = Convert.ToString(speed);

        //set the health text to the move
        hpText = gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
        hpText.text = Convert.ToString(health);
        //there has to be a better way to do this :flushed:

        //get the weapon animator
        wpAnim = gameObject.transform.GetChild(3).GetComponent<Animator>();
        wpAnim.enabled = false;

        if (game.actionPlayerScript == this)
        {
            StartTurn();
        }

    }

    // Update is called once per frame
    public void OnMouseDown()
    {
        if (state == "free")
        {
            if (game.playerWithAction == gameObject)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, LayerMask.GetMask("ground"));

                if (hit.collider != null)
                {
                    //slow down if you are standing on a Forest Tile
                    if (hit.transform.gameObject.tag.Contains("Forest"))
                    {
                        realMove = move - 1;
                    }
                    else
                    {
                        realMove = move;
                    }
                }

                //clear before
                game.Clear();

                //check move and start game
                ClearTilesFree(realMove);
            }
        }
    }

    public void ClearTilesFree(int range = 2)
    {
        //Begin making the cloud of posible moves from the original position
        Vector3 origPos = transform.position;
        //two arrays are neccesary for the foreach loop to easily loop through one group at a time
        //a "moving position" is simply a vector2 and a float for direction in one variable
        List<MovingPosition> tiles = new List<MovingPosition>();
        List<MovingPosition> tiler2 = new List<MovingPosition>();

        //begin by finding all the tiles around the original position and adding them to the searched array 
        tiles.AddRange(CheckMoveAround360(origPos));

        //loop for each move was given, starting at one because we've already looked one away
        for (int rangeAmount = 1; rangeAmount < range; rangeAmount++)
        {
            //loop through the first array
            foreach (MovingPosition tile in tiles)
            {
                //find available tiles with the direction and position of the tiles around the origPos
                tiler2.AddRange(CheckMoveAround180(tile));
            }

            //reset the tile array to only the recently found tiles to make it not search through already found tiles
            tiles.Clear();
            //set it to the recently added tiles
            tiles.AddRange(tiler2);
        }
    }

    private List<MovingPosition> CheckMoveAround180(MovingPosition tilePosition)
    {
        //create the return value
        List<MovingPosition> foundTiles = new List<MovingPosition>();

        //create a ban list for tiles with players on them
        List<Vector3> tileBanList = new List<Vector3>();

        //make the rotation offset by one so that it is around the direction
        float startDirection = tilePosition.direction - Mathf.PI / 3;

        //loop thrice for each available direction
        for (float i = startDirection; i < startDirection + Mathf.PI * 0.9; i += Mathf.PI / 3)
        {
            //find the vector2 corresponding to the direction for the Raycast using circle math
            Vector3 hexDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i));
            //on the "ground" layer, launch a raycast on the tiles beside
            RaycastHit2D hit = Physics2D.Raycast(tilePosition.position + (hexDirection * 2), hexDirection, 0.1f, LayerMask.GetMask("mages", "ground"));

            //if a collision occurs
            if (hit.transform != null)
            {
                //quickly reference the gameObject
                GameObject hitObject = hit.transform.gameObject;

                //if it is a barrier, make it red and do not add it to the return
                if (hitObject.tag.Contains("Wall"))
                {
                    hitObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 1);
                }
                else if (hitObject.tag.Contains("Player"))
                {
                    //remove this tile from being change colorable
                    tileBanList.Add(hitObject.transform.position);

                }
                else if (hitObject.tag.Contains("Tile") && !tileBanList.Contains(hitObject.transform.position)) //otherwise if it's a viable tile
                {
                    //make it green and interactable
                    if (hitObject.tag.Contains("Forest"))
                    {
                        hitObject.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else
                    {
                        hitObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 255, 1);
                    }

                    hitObject.GetComponent<BoardTile>().moveable = true;

                    //find the position and rotation of the tile raycast
                    Vector3 position = hit.transform.position;
                    MovingPosition tileDoubleVector = new MovingPosition(position, i);

                    //add the found tile to the array!
                    foundTiles.Add(tileDoubleVector);
                }
            }
        }
        //return
        return foundTiles;
    }

    private List<MovingPosition> CheckMoveAround360(Vector3 position)
    {
        //create return value
        List<MovingPosition> foundTiles = new List<MovingPosition>();

        //create a ban list for tiles with players on them
        List<Vector3> tileBanList = new List<Vector3>();

        for (float i = 0; i < Mathf.PI * 1.95; i += Mathf.PI / 3)
        {
            //search through all 6 directions
            Vector3 hexDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i));
            RaycastHit2D hit = Physics2D.Raycast(position + (hexDirection * 2), hexDirection, 0.1f, LayerMask.GetMask("ground", "mages"));

            //on hit
            if (hit.transform != null)
            {
                //do the same thing as above in the 180
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.tag.Contains("Wall"))
                {
                    hitObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 1);
                }
                else if (hitObject.tag.Contains("Player"))
                {
                    //if it is a player...
                    //BAN THETILE
                    tileBanList.Add(hitObject.transform.position);

                }
                else if (hitObject.tag.Contains("Tile") && !tileBanList.Contains(hitObject.transform.position))
                {
                    if (hitObject.tag.Contains("Forest"))
                    {
                        hitObject.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else
                    {
                        hitObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 255, 1);
                    }

                    hitObject.GetComponent<BoardTile>().moveable = true;

                    Vector3 hitPosition = hit.transform.position;
                    MovingPosition tileDoubleVector = new MovingPosition(hitPosition, i);

                    //add the found tile to the array!
                    foundTiles.Add(tileDoubleVector);
                }
            }
        }
        //return
        return foundTiles;
    }

    public void StartTurn()
    {
        game.Clear();
        game.blueTeamWithAction = blueTeam;
        state = "debuffing";

        List<Debuff> dyingEffects = new List<Debuff>();

        foreach (Debuff effect in StatusEffects)
        {
            //trigger all debuffs at the start of the turn
            effect.TriggerEffect(this);

            if (effect.remove == true)
            {
                dyingEffects.Add(effect);
            }
        }

        foreach (Debuff removeEffect in dyingEffects)
        {
            StatusEffects.Remove(removeEffect);
        }


        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = whiteSprt;

        if (frozen == true)
        {
            game.PlayAnimation(wpAnim, "unfreeze");
        }
        else
        {
            state = "free";
        }

    }

    public void EndTurn()
    {
        state = "free";
        if (dead == false)
        {
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = blueTeam ? blueSprt : redSprt;
        }
    }


    public List<GameObject> RangeAttack(int range, bool directional = false, bool pierce = false, Color hitColor = default)
    {
        if (hitColor == default)
        {
            hitColor = Color.red;
        }
        List<GameObject> tilesInRange = new List<GameObject>();

        if (directional == true)
        {
            List<RaycastHit2D> hits = RaycastAllAround(range, 3);
            foreach (RaycastHit2D hit in hits)
            {
                tilesInRange.Add(hit.transform.gameObject);
            }
        }
        else
        {
            //testing out a new method here: search through an array for every tile within the distance of an existing tile?
            List<GameObject> tempTiles = new List<GameObject>();
            tempTiles.AddRange(game.boardGenerator.activeTiles);
            List<GameObject> realTiles = new List<GameObject>();

            //remove every wall and tiles that are farther than the range from the array
            tempTiles.RemoveAll(tile => tile.tag.Contains("Wall"));

            //for (1 to Range)
            for (int i = 0; i < range; i++)
            {
                if (i == 0)
                {
                    realTiles.AddRange(tempTiles.FindAll(tile => Vector3.Distance(tile.transform.position, transform.position) < 3));
                }
                else
                {
                    List<GameObject> foundTiles = new List<GameObject>();
                    foreach (GameObject tile in realTiles)
                    {
                        foundTiles.AddRange(tempTiles.FindAll(tempTile => Vector3.Distance(tile.transform.position, tempTile.transform.position) < 3));
                    }
                    realTiles.AddRange(foundTiles);
                }
                tempTiles.RemoveAll(tile => realTiles.IndexOf(tile) >= 0);
            }

            tilesInRange.AddRange(realTiles);

        }

        //remove your tile from being hittable
        tilesInRange.Remove(tilesInRange.Find(tile => tile.transform.position == transform.position));

        List<GameObject> hittableMages = new List<GameObject>();

        foreach (GameObject tile in tilesInRange)
        {
            GameObject mage = game.activePlayers.Find(mage => mage != null && mage.transform.position == tile.transform.position);
            if (mage != null)
            {
                tile.GetComponent<BoardTile>().shootable = true;
                tile.GetComponent<SpriteRenderer>().color = hitColor;
                mage.GetComponent<CircleCollider2D>().enabled = false;
                hittableMages.Add(mage);
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
        }

        return hittableMages;
    }

    public void TakeHit(int damage = 1, Vector3 direction = default(Vector3), int knockback = 0)
    {
        health -= damage;
        hpText.text = Convert.ToString(health);

        if (knockback > 0)
        {
            GameObject knockbackTile = safeKnockbackTile(knockback, direction);
            if (knockbackTile != null)
            {
                //if there is a tile which: when added knockback doesn't run into a wall, does not contain "wall" in its tag, and doesn't knockback into a mage
                if (knockbackTile.tag.Contains("Player") && knockbackTile != gameObject)
                {
                    //funny knockback chain
                    knockbackTile.GetComponent<Mage>().TakeHit(1, direction, 1);
                    TakeHit(1);
                }
                else if (knockbackTile.tag.Contains("Tile") && !knockbackTile.tag.Contains("Wall"))
                {
                    //if there is a tile that is not a wall on the knockbacked position
                    transform.position -= direction * knockback;
                }
            }
        }

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Send singular raycasts around in all directions
    /// </summary>
    /// <param name="precision">How many rays to make around the circle</param>
    /// <param name="range">The lenght of the rays</param>
    /// <returns></returns>
    public List<RaycastHit2D> RaycastAround(int precision = 6, int range = 1)
    {
        List<RaycastHit2D> rays = new List<RaycastHit2D>();

        game.Clear();

        for (float i = 0; i < Mathf.PI * 1.9; i += Mathf.PI / precision)
        {
            //search through all 6 directions
            Vector3 hexDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i));
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (hexDirection * 2), hexDirection, (range - 1) * 2 + 0.1f, LayerMask.GetMask("wall", "mages"));

            //if it exists hit, add it to the array
            if (hit.collider != null) rays.Add(hit);
        }

        return rays;
    }

    public List<RaycastHit2D> RaycastAllAround(int range = 2, int precision = 6)
    {
        List<RaycastHit2D> rays = new List<RaycastHit2D>();

        game.Clear();

        for (float i = 0; i < Mathf.PI * 1.9; i += Mathf.PI / 3)
        {
            Vector3 hexDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + (hexDirection * 2), hexDirection, (range - 1) * 2 + 0.1f, LayerMask.GetMask("ground"));
            rays.AddRange(hits);
        }

        return rays;
    }

    GameObject safeKnockbackTile(int kb, Vector3 dir)
    {
        GameObject safe;

        Vector3 knockbackPosition = transform.position - kb * dir;
        Debug.Log(knockbackPosition);

        //instead of using raycasts, this searches through the lists with the active things 
        //mainly just to learn how to do it, i have no idea which one is more efficient.
        GameObject mage = game.activePlayers.Find(mage => mage != null && mage.transform.position == knockbackPosition);
        GameObject wall = game.boardGenerator.activeTiles.Find(tile => tile.transform.position == knockbackPosition && tile.tag.Contains("Wall"));
        GameObject ground = game.boardGenerator.activeTiles.Find(tile => tile.transform.position == knockbackPosition);

        if (mage != null)
        {
            safe = mage;
        }
        else if (wall != null)
        {
            safe = wall;
        }
        else
        {
            safe = ground;
        }

        return safe;
    }

    public void ProcessMove(Vector3 destination)
    {
        //reset the board tiles
        game.Clear();

        //play the anim
        game.PlayAnimation(anim, "MageTP");

        //find how many children
        int childrenNum = transform.childCount;

        //make every child invisible
        for (int i = 0; i < childrenNum; i++)
        {
            game.actionPlayerScript.transform.GetChild(i).gameObject.SetActive(false);
        }

        //set the destination to the players destination
        game.destination = destination;

        //stop the player who used their action from doing anything
        state = "moving";
        game.playerWithAction = null;
    }

    public void ProcessAttack(Vector3 destination, SpellCard spell)
    {

        //reset
        game.Clear();

        //quick reference the weapon anim
        GameObject weapon = wpAnim.gameObject;
        GameObject target;
        string animation = spell.spellAnim;
        Vector3 directionOfAttack;

        if (spell.hitAll == false)
        {
            //set the rotation of the weapon
            directionOfAttack = weapon.transform.position - destination;
            weapon.transform.rotation = Quaternion.LookRotation(directionOfAttack, new Vector3(0, 0, 1));
            Quaternion rotation = weapon.transform.rotation;
            rotation.x = 0;
            rotation.y = 0;
            weapon.transform.rotation = rotation;
            game.hitDirection = directionOfAttack;

            target = spell.targets.Find(mage => mage.transform.position == destination);

            //hit em
            target.GetComponent<Mage>().TakeHit(spell.damage, directionOfAttack, spell.knockback);
            spell.ExtraEffects(gameObject, target);
        }
        else
        {
            foreach (GameObject enemy in spell.targets)
            {
                //find the angle of attack (for knockback)
                directionOfAttack = weapon.transform.position - enemy.transform.position;
                //hit em
                enemy.GetComponent<Mage>().TakeHit(spell.damage, directionOfAttack, spell.knockback);
                spell.ExtraEffects(gameObject, enemy);
            }
        }


        //turn on the animator (it'll turn itslef off once complete)
        game.PlayAnimation(wpAnim, spell.spellAnim);

        //clear again afterwards so the hitboxes never die?
        game.Clear();
    }

    public void AddEffect(int amount, int effect)
    {
        for (int i = 0; i < amount; i++)
        {
            //make a lil effect
            GameObject lilEffect = Instantiate(fx[effect]);
            lilEffect.transform.SetParent(transform, false);

            Vector3 oldPos = lilEffect.transform.position;
            oldPos.x += Random.Range(-0.8f, 0.8f);
            oldPos.y += Random.Range(-0.8f, 0.8f);
            lilEffect.transform.position = oldPos;
        }
    }

    void Die()
    {
        if (game.activePlayers.IndexOf(gameObject) < game.playerActNum)
        {
            game.playerActNum--;
        }

        //Death


        //if they die on their own turn, progress to the next turn
        if (game.actionPlayerScript == this)
        {
            game.ActionComplete();
        }

        gameObject.SetActive(false);
        transform.position = new Vector3(12, 12, 12);
        dead = true;
        Destroy(gameObject);
        
    }
}
