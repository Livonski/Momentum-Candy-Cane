using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContext
{
    public Movable _movable;
    public Hand _hand;

    public void AwaitDirectionChoice(Action<TurnDirection> onChosen, Vector2 playerPosition, Vector2Int forward)
    {
        UIRequestManager.Instance.RequestDirectionChoice(onChosen, playerPosition, forward);
    }

    public void AwaitPositionChoice(Action<Vector2Int> onChosen)
    {
        UIRequestManager.Instance.RequestPositionChoise(onChosen);
    }

    public CardContext(Movable movable, Hand hand)
    {
        _movable = movable;
        _hand = hand;
    }
}
