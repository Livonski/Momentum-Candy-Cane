using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private List<CardData> _avaliableCards;
    [SerializeField] private int _initialChristmasSpirit;
    public int _christmasSpirit { get; private set; }

    private Deck _deck;
    private List<Card> _hand;
    private CardContext _context;

    private void Start()
    {
        _hand = new List<Card>();
        Movable movable = GetComponent<Movable>();
        _context = new CardContext(movable, this);

        _deck = new Deck(_avaliableCards);
        _christmasSpirit = _initialChristmasSpirit;
    }

    public void DrawCard()
    {
        Card newCard = _deck.DrawCard();
        _hand.Add(newCard);
        Debug.Log("Drawn new card");
        PrintCurrentCards();
    }

    public void PlayCard(int cardID)
    {
        if(cardID > (_hand.Count - 1))
            return;
        _hand[cardID].PlayCard(_context);
        _hand.RemoveAt(cardID);
    }

    public void DecreaseChristmasSpirit(int amount)
    {
        if (_christmasSpirit - amount < 0)
            return;
        _christmasSpirit -= amount;
    }

    private void PrintCurrentCards()
    {
        Debug.Log("Current cards");
        for (int i = 0; i < _hand.Count; i++)
        {
            Debug.Log($"Card {i} {_hand[i].GetCardData().name}");
        }
    }
}
