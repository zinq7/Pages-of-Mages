using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deck
{
    public string deckName;
    public List<string> cards;

    public deck(List<string> crads, string name)
    {
        cards = crads;
        deckName = name;
    }
}
