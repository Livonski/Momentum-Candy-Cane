using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContext
{
    public Movable _movable;
    public Hand _hand;

    public CardContext(Movable movable, Hand hand)
    {
        _movable = movable;
        _hand = hand;
    }
}
