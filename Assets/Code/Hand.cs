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

    //TMP stuff
    [SerializeField] private HandView _handView;

    private void Start()
    {
        _hand = new List<Card>();
        Movable movable = GetComponent<Movable>();
        _context = new CardContext(movable, this);

        _deck = new Deck(_avaliableCards);
        _christmasSpirit = _initialChristmasSpirit;

        //TMP stuff
        _handView.Initialize(this);
    }

    public List<Card> GetCards()
    {
        return _hand;
    }

    public void DrawCard()
    {
        _hand.Add(_deck.DrawCard());
        PrintCurrentCards();
        OnHandChanged?.Invoke(_hand);
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

    private void PrintCurrentCards()
    {
        Debug.Log("Current cards");
        for (int i = 0; i < _hand.Count; i++)
        {
            Debug.Log($"Card {i} {_hand[i].GetCardData().name}");
        }
    }
}
