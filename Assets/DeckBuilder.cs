using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> activeDeck = new List<GameObject>();
    public static DeckBuilder instance;
    public GameObject horizontalList;
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
    }

    void FormatCard(ref GameObject card)
    {
        card.transform.Rotate(0, 0, 90);
        card.transform.localScale = new Vector3(cardWidth, cardHeight, 1);
        card.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        card.transform.GetChild(0).gameObject.AddComponent<AddDeckCard>();
        Debug.Log("added the script");
    }

    void AddCard(GameObject card)
    {
        Debug.Log("Gaming");
        Debug.Log(card);
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
        int cellNum = (int)Mathf.Floor(cardNum / 6f);

        if (cardNum % 6 == 0)
        {
            horizontal = Instantiate(horizontalList);
            horizontal.transform.SetParent(scrollArea.transform, false);
        }

        card.transform.SetParent(horizontal.transform, false);
    }
}
