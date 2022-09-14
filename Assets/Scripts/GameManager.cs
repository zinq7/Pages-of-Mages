/*
 * Description: The second biggest file, stores everything that needs to be saves between levels and keeps track of turns
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
using UnityEngine.SceneManagement;
using DeckExporter;

public class GameManager : MonoBehaviour
{
    public GameObject mage;
    public List<GameObject> startHand = new List<GameObject>();
    public List<Vector3> spawnPos = new List<Vector3>() { new Vector3(-5, 4), new Vector3(-5, 0), new Vector3(-5, -4) };
    public BoardGenerator boardGenerator;
    public GameObject turnTransitionScreen, endTurnButton, blueDeckObj, redDeckObj, redCards, blueCards;

    [NonSerialized] public bool endTurn;
    [NonSerialized] public Mage actionPlayerScript;
    [NonSerialized] public int playerActNum = 0;
    [NonSerialized] public string playerState = "";
    [NonSerialized] public Vector3 destination;
    [NonSerialized] public Vector2 hitDirection;
    [NonSerialized] public static GameManager instance = null;
    [NonSerialized] public bool blueTeamWithAction;
    [NonSerialized] public GameObject hitPlayer, playerWithAction;
    [NonSerialized] public SpellCard primedSpell;
    [NonSerialized] public int turnCounter = 1;
    public List<GameObject> loadOrderPlayers = new List<GameObject>(), blueHand = new List<GameObject>(),
        activePlayers = new List<GameObject>(), redHand = new List<GameObject>(), activeHand = new List<GameObject>(),
        blueDeck = new List<GameObject>(), redDeck = new List<GameObject>();
    [NonSerialized] public List<Animator> playingAnimations = new List<Animator>();


    // Awake is called as soon as the game rusn
    void Awake()
    {
        //do some internet code so that their is only ever one GameManager instance, which can always be referenced
        if (instance == null)
        {
            //if this is the first time this GameManager has been created, call the SpecialFirstCall function where stuff happens
            instance = this;
            SpecialFirstCall();
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        //make it so it doesn't die upon loading a new stage (useful)
        //DontDestroyOnLoad(gameObject);
    }

    void SpecialFirstCall()
    {
        //what happens when the first GameManager is created
        Debug.Log("hello world");

        int counter = 0;       

        //generate the board
        boardGenerator.GenerateBoard(new List<int> { 6, 5, 6, 5, 6 }, 20, 3, 1);

        //load the blue deck
        blueDeck = DeckExporter.DeckExporter.LoadDeckFile(true);
        Debug.Log(blueDeck.Count);
        redDeck = DeckExporter.DeckExporter.LoadDeckFile(false);
        Debug.Log(blueDeck.Count);

        //add blue and red hands
        foreach (GameObject card in startHand)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject newCard = Instantiate(card, blueCards.transform);
                newCard.transform.position = new Vector3(-7.75f, 1.55f * (counter - 1), 0); //1.55 x is the spacing, 1.5 for the card, .05 buffer
                newCard.GetComponent<PlayingCard>().team = "blue";

                if (i == 1)
                {
                    Vector3 pos = newCard.transform.position;
                    newCard.transform.position = new Vector2(-pos.x, -pos.y);

                    Vector3 euler = newCard.transform.eulerAngles;
                    euler.z = Mathf.Lerp(0, 180f, 1);
                    newCard.transform.eulerAngles = euler;


                    newCard.GetComponent<PlayingCard>().team = "red";
                    redHand.Add(newCard);
                    newCard.transform.SetParent(redCards.transform);
                }
                else
                {
                    blueHand.Add(newCard);
                }
            }
            counter++;
        }

        //create the cards (but make them invisible)
        for (int i = 0; i < redDeck.Count; i++)
        {
            FormatCard(redDeck[i], redCards.transform, ref redDeck, i, redDeckObj.transform.position, "red", false);
        }

        //repeat for blue
        for (int i = 0; i < blueDeck.Count; i++)
        {
            FormatCard(blueDeck[i], blueCards.transform, ref blueDeck, i, blueDeckObj.transform.position, "blue", true);
            
        }

        //create all six mages
        for (int i = 0; i < 6; i++)
        {
            //make one
            GameObject mage1 = Instantiate(mage, new Vector3(0, 0), new Quaternion(0, 0, 0, 0));

            //add quick reference to the Mage component
            Mage mage1Stats = mage1.GetComponent<Mage>();


            //for now randomize stats
            mage1Stats.health = (int)Random.Range(2f, 6f);
            mage1Stats.speed = 8 - mage1Stats.health;

            //the half on blue
            if (i / 3 == 0)
            {
                //blue to true and spawn them at a spawnPos
                mage1Stats.blueTeam = true;
                mage1.transform.position = spawnPos[i];
            }
            else
            {
                //spawn them at the reverse spawnpos (negative)
                mage1.transform.position = -spawnPos[i % 3];
            }

            //add the mages
            activePlayers.Add(mage1);
        }
        //create the load order players array
        loadOrderPlayers.AddRange(activePlayers);

        //order the list by the mages speed, reversed (index 0 = most speed)
        activePlayers = activePlayers.OrderBy(mage => mage.GetComponent<Mage>().speed).Reverse().ToList();

        //ya
        playerActNum = 0;
        //simulate starting the game
        playerWithAction = activePlayers[0];
        actionPlayerScript = playerWithAction.GetComponent<Mage>();
        actionPlayerScript.Start();
        GameStartTurn(actionPlayerScript, playerWithAction);
    }


    /// <summary>
    /// Call once a character has used their action
    /// </summary>
    public void ActionComplete()
    {
        //make the next turn button visible and do something
        endTurnButton.SetActive(true);
        StartCoroutine(waitWall());
    }

    public void EndTurnTrue()
    {
        endTurn = true;
        endTurnButton.SetActive(false);
    }

    IEnumerator waitWall()
    {
        yield return new WaitUntil(() => endTurn == true);

        //add one to the active player counter
        playerActNum++;

        //Mod it by 6
        playerActNum %= activePlayers.Count;

        //once looped up the turn counter (neeeds fix)
        if (playerActNum == 0)
        {
            activePlayers.RemoveAll(mage => mage == null);
        }

        //unshow
        actionPlayerScript.EndTurn();

        if (activePlayers[playerActNum] != null)
        {
            //set the player who's active to be active
            playerWithAction = activePlayers[playerActNum];
        }
        else
        {
            waitWall();
        }

        endTurn = false;

        //link the script
        actionPlayerScript = playerWithAction.GetComponent<Mage>();

        //set the wall to appear gradually
        turnTransitionScreen.SetActive(true);
        turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().BuildWall();
        Color color = turnTransitionScreen.GetComponent<SpriteRenderer>().color;
        turnTransitionScreen.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);

        //change the te
        blueTeamWithAction = actionPlayerScript.blueTeam;
        turnTransitionScreen.GetComponent<SetText>().ChangeTexts("Player " + Convert.ToString(blueTeamWithAction ? 1 : 2) + "'s Turn");
    }

    public void NextTurn()
    {
        //do the essential gameManager stuff
        GameStartTurn(actionPlayerScript, playerWithAction);
    }

    void GameStartTurn(Mage script, GameObject mage)
    {
        activeHand = script.blueTeam ? redHand : blueHand;
        List<GameObject> altList = script.blueTeam ? blueHand : redHand;

        foreach (GameObject card in activeHand)
        {
            card.GetComponent<PlayingCard>().TurnCardToBack();
        }

        foreach (GameObject card in altList)
        {
            card.GetComponent<PlayingCard>().TurnCardToFront();
        }

        //show that it is that persons action
        script.StartTurn();
    }

    public void PlayAnimation(Animator anim, string animName)
    {
        if (anim.HasState(0, Animator.StringToHash(animName)))
        {
            Debug.Log("hasState");
        }
        anim.enabled = true;
        anim.Play(Animator.StringToHash(animName));
        playingAnimations.Add(anim);
    }

    public void FinishAnimation(Animator anim)
    {
        anim.enabled = false;
        playingAnimations.Remove(anim);
    }

    public void Clear()
    {
        boardGenerator.ResetTiles();
        foreach (GameObject mage in activePlayers)
        {
            if (mage != null)
            {
                mage.GetComponent<CircleCollider2D>().enabled = true;
            }

        }
    }

    public GameObject DeckCard(bool blue)
    {
        GameObject crd = null;
        if (blue)
        {
            //while (card == null)
            //{
            crd = blueDeck[Random.Range(0, blueDeck.Count - 1)];
            //}
            blueDeck.Remove(crd);
        }
        else
        {
            //while (card == null)
            //{
            crd = redDeck[Random.Range(0, redDeck.Count - 1)];
            //}
            redDeck.Remove(crd);
        }

        crd.SetActive(true);

        return crd;
    }

    public void FormatCard(GameObject cardType, Transform cardParent, ref List<GameObject> deck, int i, Vector3 deckPosition, string team, bool blue)
    {
        //format the card to the deck
        GameObject card = Instantiate(cardType, cardParent);
        card.SetActive(false);
        deck.RemoveAt(i);
        deck.Insert(i, card);
        card.transform.position = deckPosition;
        card.GetComponent<PlayingCard>().team = team;
        if (!blue) card.transform.Rotate(0f, 0f, 180f); //if it's red rotate it so...
    }

    public void Shuffle<T>(IList list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = (T)list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void DrawNewCard(GameObject prevCard)
    {
        //find the deck
        GameObject deck = blueTeamWithAction ? blueDeckObj : redDeckObj;
        //create the card at the deck
        GameObject card;
        //this should not be one line but whatever
        if (blueTeamWithAction) { blueHand.Remove(prevCard); card = DeckCard(true); blueDeck.Add(prevCard); }
        else { redHand.Remove(prevCard); card = DeckCard(false); redDeck.Add(prevCard); }

        if (blueTeamWithAction) { blueHand.Add(card); }
        else { redHand.Add(card); }


        //find the destination to be
        _dest = prevCard.transform.position;
        _dest.x = 7.75f * Mathf.Sign(_dest.x);
        _moveCard = true;
        _card = card;
        _speed = Vector3.Distance(_dest, _card.transform.localPosition) / 0.5f;

        prevCard.transform.position = deck.transform.position;

        _card.GetComponent<PlayingCard>().TurnCardToBack();
        card.SetActive(true);

        //update
        Update();
    }

    //declared only for the update right below, ok?
    Vector3 _dest;
    GameObject _card;
    bool _moveCard;
    float _speed;
    int _turnCard;

    private void Update()
    {
        if (_moveCard)
        {

            _card.transform.localPosition = Vector3.MoveTowards(_card.transform.localPosition, _dest, _speed * Time.deltaTime);

            if (_card.transform.localPosition == _dest)
            {
                _moveCard = false;
                _turnCard = 120;
            }
        }
        else if (_turnCard > 0)
        {
            _card.transform.Rotate(3, 0, 0);
            _turnCard--;

            if (_turnCard == 60)
            {
                _card.GetComponent<PlayingCard>().TurnCardToFront();
            }
        }
    }
}

