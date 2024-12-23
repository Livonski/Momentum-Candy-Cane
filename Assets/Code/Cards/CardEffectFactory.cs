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
                return new MoveEffect(data);
            case EffectType.Turn:
                return new TurnEffect(data);
            case EffectType.Shoot:
                return new ShootEffect(data);
            case EffectType.Heal:
                return new HealEffect(data);
            case EffectType.DrawCards:
                return new DrawCardEffect(data);
            case EffectType.MoveBackwards:
                return new MoveBackwardEffect(data);
            default:
                Debug.LogWarning($"Unknown card effect {data.Type}");
                return null;
        }
    }
}