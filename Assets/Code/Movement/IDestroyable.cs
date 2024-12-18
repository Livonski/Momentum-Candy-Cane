using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyable
{
    public void TakeDamage(int damage);
    public void OnRemove();
}
