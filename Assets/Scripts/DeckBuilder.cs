using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DeckBuilder : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> activeDeck = new List<GameObject>();
    public List<GameObject> deckCardSlots = new List<GameObject>();
    public static DeckBuilder instance;
    public GameObject horizontalListObj;
    public GameObject deckCardObj;
    public GameObject deckCardArea;
    public GameObject scrollArea;
    private GameObject horizontal;

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
        //add all the cards
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            GameObject liveCard = Instantiate(card);
            cards.RemoveAt(i);
            cards.Insert(i, liveCard);

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
            card.GetComponent<Image>().color = Color.white;

        }
        //check if there's space
        else if (!CardSlot())
        {
            return;
        }
        else
        {
            activeDeck.Add(card);
            card.GetComponent<Image>().color = Color.gray;
            AddCard(card);
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
        Debug.Log(File.ReadAllText("C:/Users/16132/Documents/DataOfMages/Gamer.txt"));
        if (!File.Exists("./Build/Pages Of Mages_Data/Game.txt"))
        {
            File.CreateText("./Build/Pages Of Mages_Data/Game.txt");
        }

        string number = System.Convert.ToString(Random.Range(0f, 100f) / 100f);
        File.WriteAllText("./Build/Pages Of Mages_Data/Game.txt", number);

        Debug.Log(File.ReadAllText("./Build/Pages Of Mages_Data/Game.txt"));
    }

    public void LoadDeck()
    {

    }
}
