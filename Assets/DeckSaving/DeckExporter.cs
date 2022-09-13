using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;

namespace DeckExporter
{
    public class DeckExporter
    {

        static List<GameObject> loadedDeck = new List<GameObject>();
        public static void SaveDeckFile(List<CardID> cards)
        {
            loadedDeck.Clear();

            List<GameObject> deck = CardStringToGameObject(cards);

            deck deck1 = new deck(cards, "_LastDeck");//_LastDeck is the name
            
            string jsonDeck = JsonUtility.ToJson(deck1);


            Debug.Log(jsonDeck);

            Debug.Log(deck.Count);
            Debug.Log(cards.Count);

            loadedDeck = deck;
          
            string savePath = Application.persistentDataPath;
            File.WriteAllText(savePath + "/_LastDeck.json", jsonDeck, UTF8Encoding.UTF8);
        }

        public static List<GameObject> LoadDeckFile()
        {
            List<string> savedDecks = new List<string>();
            string[] h = Directory.GetFiles(Application.persistentDataPath); //h = all files in the folder. _LastDeck should be the first
            savedDecks.AddRange(h);

            foreach (string deck in savedDecks)
            {
                if (deck.Contains(".json"))
                {
                    string name1 = deck.Replace(Application.persistentDataPath, "");
                    string name2 = name1.Replace(".json", "");
                    string name = name2.Replace("\\", "");

                    Debug.Log(name);
                }
            }

            if (loadedDeck.Count == 0)
            {
                loadedDeck = JsonIDsToDeck(Directory.GetFiles(Application.persistentDataPath)[0]);    
            }

            return loadedDeck;
        }

        public static List<int> LoadDeckFileIDs()
        {
            List<int> ids = new List<int>();
            string[] h = Directory.GetFiles(Application.persistentDataPath);
            List<string> hList = new List<string>();
            hList.AddRange(h);
            string path = hList.Find(path => path.Contains("LastDeck"));

            string jsonDeck = File.ReadAllText(path); //read all text in the json file
            deck loadedDeck = JsonUtility.FromJson<deck>(jsonDeck); //parse it into a deck file
            Debug.Log(loadedDeck.deckName);

            foreach (CardID crdID in loadedDeck.cards)
            {
                ids.Add(crdID.cardID); // add the id of the card to the return list
            }

            return ids; //return
        }

        static List<GameObject> JsonIDsToDeck(string jsonDeckPath)
        {
            string jsonDeck = File.ReadAllText(jsonDeckPath); //read all text in the json file
            Debug.Log(jsonDeck);
            deck loadedDeck = JsonUtility.FromJson<deck>(jsonDeck); //parse it into a deck file
            Debug.Log(loadedDeck);
            Debug.Log(loadedDeck.cards);

            return CardStringToGameObject(loadedDeck.cards); //return the associated objects with the names
        }

        static List<GameObject> CardStringToGameObject(List<CardID> cards)
        {
            List<string> names = new List<string>();
            foreach (CardID crd in cards)
            {
                names.Add(crd.cardName);
            }

            List<GameObject> allCards = new List<GameObject>();
            List<GameObject> deck = new List<GameObject>();
            allCards.AddRange(Resources.LoadAll<GameObject>("Cards"));

            foreach (GameObject card in allCards)
            {
                List<string> foundCards = names.FindAll(name => name == card.GetComponentInChildren<Text>().text);

                if (foundCards.Count != 0)
                {
                    //find the type of card it is
                    GameObject tempCard = allCards.Find(cardName => cardName.GetComponentInChildren<Text>().text == foundCards[0]);

                    //add it to the deck for each duplicate
                    foreach (string foundCard in foundCards)
                    {
                        deck.Add(tempCard);
                    }
                }
            }

            return deck;
        }
    }
}
