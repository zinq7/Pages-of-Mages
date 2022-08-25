using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        deckCardSlots[index].transform.GetChild(0).GetComponent<Text>().text = "Hoodle Poodel";
    }

    void RemoveCard(GameObject card)
    {

    }

    public void AddToDeck(GameObject card)
    {
        //find if you can add a card, and if so, do
        if (activeDeck.Find(crd => crd == card))
        {
            activeDeck.Remove(card);
            card.GetComponent<Image>().color = Color.white;
            AddCard(card);

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
            RemoveCard(card);
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
}
