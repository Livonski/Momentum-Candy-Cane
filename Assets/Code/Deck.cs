using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private Queue<Card> cards = new Queue<Card>();

    public Deck(List<CardData> cardDataList)
    {
        foreach (var data in cardDataList)
        {
            cards.Enqueue(new Card(data));
        }
    }

    public Card DrawCard()
    {
        return cards.Count > 0 ? cards.Dequeue() : null;
    }
}
