using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : ICardEffect
{
    private int _acceleration;

    public MoveEffect(CardEffectData data)
    {
        _acceleration = data.Value;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context._movable.AccelerateForward(_acceleration);
        TurnManager.Instance.PredictMovements();
    }
}
