using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : ICardEffect
{
    private int _acceleration;
    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        Debug.Log("Applied move effect");
    }
}
