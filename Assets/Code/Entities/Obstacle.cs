using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInitializable, IBlockable
{
    private Vector2Int _gridPosition;
    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        transform.name = data.DefaultName + data.ID;
    }
}
