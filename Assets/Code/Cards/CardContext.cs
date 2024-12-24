using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContext
{
    public Movable _movable;
    public Hand _hand;
    public Vehicle _vehicle;
    
    public bool _AI = false;
    public Vector2Int _target;
    public TurnDirection _turnDirection;

    public void AwaitDirectionChoice(Action<TurnDirection> onChosen, Vector2 playerPosition, Vector2Int forward)
    {
        if(!_AI)
        {
            UIRequestManager.Instance.RequestDirectionChoice(onChosen, playerPosition, forward);
        }
        else
        {
            onChosen.Invoke(_turnDirection);
        }
    }

    public void AwaitPositionChoice(Action<Vector2Int> onChosen, Vector2Int gridPosition)
    {
        if (!_AI)
        {
            UIRequestManager.Instance.RequestPositionChoise(onChosen, gridPosition);
        }
        else
        {
            onChosen.Invoke(_target);
        }
    }

    public CardContext(Movable movable, Hand hand, Vehicle vehicle)
    {
        _movable = movable;
        _hand = hand;
        _vehicle = vehicle;
    }
}
