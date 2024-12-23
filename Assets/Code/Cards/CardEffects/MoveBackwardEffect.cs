using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwardEffect : ICardEffect
{
    private int _acceleration;
    public MoveBackwardEffect(CardEffectData data)
    {
        _acceleration = data.Value;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context._movable.AccelerateBackward(_acceleration);
        TurnManager.Instance.PredictMovements();
    }
}
