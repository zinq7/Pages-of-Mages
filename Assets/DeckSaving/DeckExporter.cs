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
        public static void SaveDeckFile(List<string> names)
        {
            loadedDeck.Clear();

            List<GameObject> deck = CardStringToGameObject(names);

            deck deck1 = new deck(names, "hooo");//hoo is name
            string jsonDeck = JsonUtility.ToJson(deck1);


            Debug.Log(jsonDeck);

            Debug.Log(deck.Count);
            Debug.Log(names.Count);

            loadedDeck = deck;
          
            string savePath = Application.persistentDataPath;
            File.WriteAllText(savePath + "/hooo.json", jsonDeck, UTF8Encoding.UTF8);
        }

        public static List<GameObject> LoadDeckFile()
        {
            List<string> savedDecks = new List<string>();
            string[] h = Directory.GetFiles(Application.persistentDataPath);
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
                loadedDeck = jsonIDsToDeck(Directory.GetFiles(Application.persistentDataPath)[0]);    
            }

            return loadedDeck;
        }

        static List<GameObject> jsonIDsToDeck(string jsonDeckPath)
        {
            string jsonDeck = File.ReadAllText(jsonDeckPath);
            Debug.Log(jsonDeck);
            deck loadedDeck = JsonUtility.FromJson<deck>(jsonDeck);
            Debug.Log(loadedDeck);
            Debug.Log(loadedDeck.cards);

            return CardStringToGameObject(loadedDeck.cards); ;
        }

        static List<GameObject> CardStringToGameObject(List<string> names)
        {
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
