using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : ICardEffect
{
    private int _drawAmount;

    public DrawCardEffect(CardEffectData data)
    {
        _drawAmount = data.Value;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context._hand.DrawCards(_drawAmount);
    }
}
