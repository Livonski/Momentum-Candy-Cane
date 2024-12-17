using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializable
{
    void OnInitialize(Vector2Int initialPosition, int ID);
}
