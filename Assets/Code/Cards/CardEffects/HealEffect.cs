using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : ICardEffect
{
    private int _healAmount;

    public HealEffect(CardEffectData data)
    {
        _healAmount = data.Value;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context._vehicle.Heal(_healAmount);
    }
}
