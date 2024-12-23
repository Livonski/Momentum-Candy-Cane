using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private CardData _cardDataReference;
    private AudioClip _playClip;

    public Card(CardData cardDataReference)
    {
        _cardDataReference = cardDataReference;
    }

    public bool PlayCard(CardContext context)
    {
        if (_cardDataReference.Effects == null)
        {
            Debug.LogWarning($"Card data is not set");
            return false;
        }

        if(_cardDataReference.Effects == null)
        {
            Debug.LogWarning($"Card effects are not set");
            return false;
        }

        if(context._hand._christmasSpirit < _cardDataReference.SpiritCost)
        {
            //TODO: show message to player?
            Debug.Log("Not enough christmas spirit to play card");
            return false;
        }

        context._hand.DecreaseChristmasSpirit(_cardDataReference.SpiritCost);
        if(_cardDataReference.PlayClip != null) 
            SFXPlayer.Instance.PlaySFX(_cardDataReference.PlayClip);

        foreach(var effectData in _cardDataReference.Effects)
        {
            Debug.Log($"Played card {_cardDataReference.name}");
            ICardEffect effect = CardEffectFactory.CreateEffect(effectData);
            effect?.ApplyEffect(context);
        }
        return true;
    }

    public CardData GetCardData()
    {
        return _cardDataReference;
    }
}
