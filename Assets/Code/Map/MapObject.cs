using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    public int ID { get; private set; }

    public void OnInitialize(Vector2Int position, int ID)
    {
        _gridPosition = position;
        this.ID = ID;
        transform.name = "Tile " + ID;
    }
}
