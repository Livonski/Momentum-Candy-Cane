using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnDirection { Left, Right }

public class TurnEffect : ICardEffect
{
    private int _numRotations;

    public TurnEffect(CardEffectData data)
    {
        _numRotations = data.Value;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;

        //Yes it's hardcoded. Yes I don't care
        if (_numRotations == 4)
        {
            context._movable.Turn(1, _numRotations);
            return;
        }

        context.AwaitDirectionChoice((direction) =>
        {
            float side = direction == TurnDirection.Left ? -1 : 1;
            context._movable.Turn(side, _numRotations);
            TurnManager.Instance.PredictMovements();
        }, context._movable.gameObject.transform.position, context._movable.Forward);
    }
}
