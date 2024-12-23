using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private List<CardData> _avaliableCards;
    [SerializeField] private int _initialChristmasSpirit;
    public int _christmasSpirit { get; private set; }
    public event Action<List<Card>> OnHandChanged;

    private Deck _deck;
    private List<Card> _hand;
    private CardContext _context;

    private void Awake()
    {
        _hand = new List<Card>();
        Movable movable = GetComponent<Movable>();
        Vehicle vehicle = GetComponent<Vehicle>();
        _context = new CardContext(movable, this, vehicle);

        _deck = new Deck(_avaliableCards);
        _christmasSpirit = _initialChristmasSpirit;
    }

    public List<Card> GetCards()
    {
        return _hand;
    }

    public void DrawCard()
    {
        _hand.Add(_deck.DrawCard());
        OnHandChanged?.Invoke(_hand);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _hand.Add(_deck.DrawCard());
            OnHandChanged?.Invoke(_hand);
        }
    }

    public void PlayCard(int cardID)
    {
        if(cardID > (_hand.Count - 1)) return;
        bool playedCard = _hand[cardID].PlayCard(_context);
        if (playedCard)
        {
            _hand.RemoveAt(cardID);
            OnHandChanged?.Invoke(_hand);
        }
    }

    public bool PlayCard(Card card)
    {
        if(!_hand.Contains(card)) return false;
        bool playedCard = card.PlayCard(_context);
        if (playedCard)
        {
            _hand.Remove(card);
            OnHandChanged?.Invoke(_hand);
        }
        return playedCard;
    }

    public void DecreaseChristmasSpirit(int amount)
    {
        if (_christmasSpirit - amount < 0)
            return;
        _christmasSpirit -= amount;
    }

    public void IncreaseChristmasSpirit(int amount)
    {
        _christmasSpirit += amount;
    }
}
