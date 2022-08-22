/*
 * Description: The control for a all non-click inputs, currently the Opening and Closing hand action
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputControl : MonoBehaviour
{
    bool handShown = false;
    public bool waitingForClick;
    private SpriteRenderer img;
    public int wallAnim = -1;
    public GameObject handWall;
    List<Quaternion> pastRotations = new List<Quaternion>();
    List<Vector3> pastPositions = new List<Vector3>();
    List<GameObject> hand = new List<GameObject>();

    void ShowHand()
    {
        //because red and blue start at opposite orientations, checking whether the card is red is necessary
        bool redRot = false;

        //add the backpad
        handWall.gameObject.SetActive(true);

        //i know this uses the same variable twice for different things (bad), but it works 
        hand = GameManager.instance.activeHand;
        if (hand == GameManager.instance.blueHand)
        {
            hand = GameManager.instance.redHand;
            redRot = true;
        }
        else
        {
            hand = GameManager.instance.blueHand;
        }

        //clear the lists
        pastRotations.Clear();
        pastPositions.Clear();
        handShown = true;
        int counter = 0;
        //for every card in hand...
        foreach (GameObject card in hand)
        {
            pastRotations.Add(card.transform.rotation); // add rotation
            pastPositions.Add(card.transform.position); // add position
            card.transform.position = new Vector3(3.3f * counter - 4.95f, 0); //4.95 is 3.3, the spacing between cards, times 1.5
            //add the rotation so they are vertical
            if (redRot) { card.transform.Rotate(0, 0, -90); }
            else { card.transform.Rotate(0, 0, 90); }

            card.transform.localScale = new Vector3(2, 2, 2); //double the size 
            counter++;
        }
    }

    void HideHand()
    {
        //dissapear the stuff
        handWall.gameObject.SetActive(false);
        handShown = false;
        int counter = 0;

        foreach (GameObject card in hand)
        {
            //reset the positions
            card.transform.position = pastPositions[counter];
            card.transform.rotation = pastRotations[counter];
            card.transform.localScale = Vector3.one;
            counter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if they are between turns, prioratize this
        if (waitingForClick)
        {
            //on click do the transition thing
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                waitingForClick = false;
                GameManager.instance.NextTurn();
                transform.Find("TurnTransitionWall").gameObject.SetActive(false);
            }
        }
        else
        {
            //if they hit space, depending on whether the hand is there or not do the things
            if (Input.GetKeyDown(KeyCode.Space) && !handShown)
            {
                ShowHand();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && handShown)
            {
                HideHand();
            }
        }

    }

    private void Start()
    {
        //faster
        img = transform.Find("TurnTransitionWall").gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        //phase the wall into appearance
        if (wallAnim > 0)
        {
            wallAnim -= 3;
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 1f);
            if (wallAnim < 0) wallAnim = 0;
        }
        else if (wallAnim == 0)
        {
            waitingForClick = true;
            wallAnim--;
        }
    }

    public void BuildWall()
    {
        wallAnim = 1;
        if (handShown) HideHand();
    }
}
