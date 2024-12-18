using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEffect : ICardEffect
{
    private int _acceleration;
    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        Debug.Log("Applied turn effect");
    }
}
