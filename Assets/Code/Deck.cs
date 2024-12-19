using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private Queue<Card> _cards = new Queue<Card>();
    private List<CardData> _avaliableCards = new List<CardData>();
    public Deck(List<CardData> cardDataList)
    {
        _avaliableCards = cardDataList;
        AssignDeck();
    }

    private void AssignDeck()
    {
        _cards.Clear();
        foreach (var data in _avaliableCards)
        {
            _cards.Enqueue(new Card(data));
        }
    }

    public Card DrawCard()
    {
        if(_cards.Count == 0)
            AssignDeck();
        return _cards.Dequeue();
    }
}
