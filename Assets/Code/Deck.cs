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
        List<Card> cardList = new List<Card>();
        foreach (var data in _avaliableCards)
        {
            for (int i = 0; i < data.Rarity; i++)
            {
                cardList.Add(new Card(data));
            }
        }

        Shuffle(cardList);

        foreach (var card in cardList)
        {
            _cards.Enqueue(card);
        }
    }

    private void Shuffle(List<Card> cardList)
    {
        System.Random rnd = new System.Random();
        for (int i = cardList.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            Card temp = cardList[i];
            cardList[i] = cardList[j];
            cardList[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if(_cards.Count == 0)
            AssignDeck();
        return _cards.Dequeue();
    }
}
