using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DeckExporter;

public class DeckBuilder : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> activeDeck = new List<GameObject>();
    public List<CardID> activeDeckIDs = new List<CardID>(); //list of every card and position you have clicked
    public List<GameObject> deckCardSlots = new List<GameObject>();
    List<GameObject> tempCards = new List<GameObject>();
    public static DeckBuilder instance;
    public GameObject horizontalListObj;
    public GameObject deckCardObj;
    public GameObject deckCardArea;
    public GameObject scrollArea;
    public GameObject tooManyCardsObj;
    public GameObject colorAlternator;

    private GameObject horizontal;

    public bool editBlueDeck;
    public float cardWidth;
    public float cardHeight;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this, 0);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        editBlueDeck = true; //when loading the scene, begin editing bluedeck

        //add all the cards
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            GameObject liveCard = Instantiate(card);
            liveCard.GetComponentInParent<PlayingCard>().id = i;
            cards.RemoveAt(i);
            cards.Insert(i, liveCard);
            tempCards.Add(liveCard); //add the card part to the array

            FormatCard(ref liveCard);

            FindCardHome(liveCard, i);
        }

        

        horizontal.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1; //scroll to the top

        //add the deck building zone
        for (int i = 0; i < 12; i++)
        {
            GameObject obj = Instantiate(deckCardObj, deckCardArea.transform);
            deckCardSlots.Add(obj);
        }

        List<int> clickIDs = DeckExporter.DeckExporter.LoadDeckFileIDs(); //load the previously selected cards
        ClickTempCardsFromID(clickIDs); //Click them

        
    }

    void FormatCard(ref GameObject card)
    {
        card.transform.Rotate(0, 0, 90);
        card.transform.localScale = new Vector3(cardWidth, cardHeight, 1);
    }

    void AddCard(GameObject card)
    {
        int index = activeDeck.IndexOf(card);
        string name = card.name;
        deckCardSlots[index].transform.GetChild(0).GetComponent<Text>().text = name;
        deckCardSlots[index].GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
    }

    void RemoveCard(int index)
    {
        for (int i = index; i < activeDeck.Count; i++)
        {
            if (i == deckCardSlots.Count -1)
            {
                deckCardSlots[i].GetComponentInChildren<Text>().text = "<empty>";
                deckCardSlots[i].GetComponent<Image>().color = new Color(0.45f, 0.45f, 0.45f, 0.6f);
            }
            else
            {
                deckCardSlots[i].GetComponentInChildren<Text>().text = deckCardSlots[i + 1].GetComponentInChildren<Text>().text;
                deckCardSlots[i].GetComponent<Image>().color = deckCardSlots[i + 1].GetComponent<Image>().color;
            } 
        }
    }

    public void AddToDeck(GameObject card)
    {
        //find if you can add a card, and if so, do
        if (activeDeck.Find(crd => crd == card))
        {
            RemoveCard(activeDeck.IndexOf(card));
            activeDeck.Remove(card);
            activeDeckIDs.Remove(activeDeckIDs.Find(CardID => CardID.cardID == card.GetComponentInParent<PlayingCard>().id));
            card.GetComponent<Image>().color = Color.white;
            card.GetComponentInParent<PlayingCard>().clicked = false;

        }
        //check if there's space
        else if (!CardSlot())
        {
            return;
        }
        else
        {
            activeDeck.Add(card);
            activeDeckIDs.Add(new CardID(card.GetComponentInParent<PlayingCard>().id, card.GetComponentInChildren<Text>().text)); //the first text found is the title on the card
            card.GetComponent<Image>().color = Color.gray;
            AddCard(card);
            card.GetComponentInParent<PlayingCard>().clicked = true;
        }

       
    }

    public bool CardSlot()
    {
        //simple
        if (activeDeck.Count >= 12)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void FindCardHome(GameObject card, int cardNum)
    {
        if (cardNum % 5 == 0)
        {
            horizontal = Instantiate(horizontalListObj);
            horizontal.transform.SetParent(scrollArea.transform, false);
        }

        card.transform.SetParent(horizontal.transform, false);
    }

    public void SaveDeck()
    {
        if (activeDeck.Count < 12)
        {
            //shows when theres too few cards
            tooManyCardsObj.GetComponent<Dissapear>().ShowMessage();
            return;
        }

        DeckExporter.DeckExporter.SaveDeckFile(activeDeckIDs, editBlueDeck);
    }

    public void LoadDeck()
    {
        //not actually the load deck but a debug
    }

    public void ChangeColor()
    {
        //to begin: unclick every card
        foreach (GameObject card in tempCards)
        {
            if (card.GetComponentInParent<PlayingCard>().clicked == true)
            {
                AddToDeck(card.transform.GetChild(0).gameObject);
            }
        }

        editBlueDeck = !editBlueDeck; //flip the deck that's being edited

        Text txt = colorAlternator.GetComponent<Text>();
        txt.text = editBlueDeck ? "blue" : "red";
        txt.color = editBlueDeck ? new Color(0.6117647f, 8431373f, 8392158f) : new Color(0.8392158f, 0.6039216f, 0.6117647f);

        //now click every card, doi?
        List<int> clickIDs = DeckExporter.DeckExporter.LoadDeckFileIDs(editBlueDeck); //load the previously selected cards

        ClickTempCardsFromID(clickIDs);
    }

    public void ClickTempCardsFromID(List<int> ids)
    {
        //search for cards with matching ids and click em'
        foreach (GameObject clkCrd in tempCards)
        {
            if (ids.Exists(lid => lid == clkCrd.GetComponentInParent<PlayingCard>().id))
                //if a card with the same ID exists, click it
            {
                AddToDeck(clkCrd.transform.GetChild(0).gameObject);
            }
        }
    }
}
