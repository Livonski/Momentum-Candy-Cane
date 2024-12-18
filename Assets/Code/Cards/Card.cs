using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    [SerializeField] private CardData _cardDataReference;

    public Card(CardData cardDataReference)
    {
        _cardDataReference = cardDataReference;
    }

    public void PlayCard(CardContext context)
    {
        if (_cardDataReference.Effects == null)
        {
            Debug.LogWarning($"Card data is not set");
            return;
        }

        if(_cardDataReference.Effects == null)
        {
            Debug.LogWarning($"Card effects are not set");
            return;
        }

        if(context._hand._christmasSpirit < _cardDataReference.SpiritCost)
        {
            //TODO: show message to player?
            Debug.Log("Not enough christmas spirit to play card");
            return;
        }

        context._hand.DecreaseChristmasSpirit(_cardDataReference.SpiritCost);

        foreach(var effectData in _cardDataReference.Effects)
        {
            Debug.Log($"Played card {_cardDataReference.name}");
            ICardEffect effect = CardEffectFactory.CreateEffect(effectData);
            effect?.ApplyEffect(context);
        }
    }
}
