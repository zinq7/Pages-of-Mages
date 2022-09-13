using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deck
{
    public string deckName;
    [field: SerializeField]
    public List<CardID> cards;

    public deck(List<CardID> crads, string name)
    {
        cards = crads;
        deckName = name;
    }
}
