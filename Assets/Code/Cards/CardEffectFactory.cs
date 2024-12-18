using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect CreateEffect(CardEffectData data)
    {
        switch(data.Type)
        {
            case EffectType.Move:
                return new MoveEffect();
            case EffectType.Turn:
                return new TurnEffect();
            default:
                Debug.LogWarning($"Unknown card effect {data.Type}");
                return null;
        }
    }
}