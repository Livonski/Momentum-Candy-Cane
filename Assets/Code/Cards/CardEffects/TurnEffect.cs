using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnDirection { Left, Right }

public class TurnEffect : ICardEffect
{
    private int _acceleration;

    public TurnEffect(CardEffectData data)
    {

    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context.AwaitDirectionChoice((direction) =>
        {
            float side = direction == TurnDirection.Left ? -1 : 1;
            context._movable.Turn(side);
        }, context._movable.gameObject.transform.position, context._movable.Forward);
    }
}
