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
        const string BLUEDECKFILE = "_LastDeck_B";
        const string REDDECKFILE = "_LastDeck_R";
        static string deckFile;
        public static void SaveDeckFile(List<CardID> cards, bool blue = true)
        {
            GetColorDeck(blue); //affect the right color

            loadedDeck.Clear();

            List<GameObject> deck = CardStringToGameObject(cards);

            deck deck1 = new deck(cards, deckFile);//_LastDeck is the name
            
            string jsonDeck = JsonUtility.ToJson(deck1);


            Debug.Log(jsonDeck);

            Debug.Log(deck.Count);
            Debug.Log(cards.Count);

            loadedDeck = deck;
          
            string savePath = Application.persistentDataPath;
            File.WriteAllText(savePath + "/" + deckFile + ".json", jsonDeck, UTF8Encoding.UTF8);
        }

        public static List<GameObject> LoadDeckFile(bool blue = true)
        {
            List<GameObject> returnDeck = new List<GameObject>();
            GetColorDeck(blue); //affect the right color

            List<string> savedDecks = new List<string>();
            string[] h = Directory.GetFiles(Application.persistentDataPath); //h = all files in the folder. _LastDeck should be the first
            savedDecks.AddRange(h);
            string lastDeckPath = "haha not a path"; //save lastDeckPath

            foreach (string deck in savedDecks)
            {
                if (deck.Contains(deckFile))
                {
                    lastDeckPath = deck; //set the 
                }
            }

            if (returnDeck.Count == 0)
            {
                returnDeck = JsonIDsToDeck(lastDeckPath); //get the lastDeck 
            }

            return returnDeck;
        }

        public static List<int> LoadDeckFileIDs(bool blue = true)
        {
            GetColorDeck(blue); //affect the right color

            List<int> ids = new List<int>();
            string[] h = Directory.GetFiles(Application.persistentDataPath);
            List<string> hList = new List<string>();
            hList.AddRange(h);
            string path = hList.Find(path => path.Contains(deckFile)); // get the lastDeck path

            if (path == null)
            {
                return ids;
            }

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
            deck loadedDeck = JsonUtility.FromJson<deck>(jsonDeck); //parse it into a deck file

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

        static void DebugPog()
        {
            Debug.Log("Pog"); //logs pog to see if something went somewhere
        }

        static void GetColorDeck(bool blue)
        {
            deckFile = (blue) ? BLUEDECKFILE : REDDECKFILE; //set the deck to the color
        }
    }
}
