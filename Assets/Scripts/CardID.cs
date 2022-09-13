using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardID
{
    public string cardName;
    public int cardID;

    public CardID(int id, string name)
    {
        //short id and card datatype
        cardName = name;
        cardID = id;
    }
}
