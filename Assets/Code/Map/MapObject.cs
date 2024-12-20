using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    public int ID { get; private set; }
    public List<GameObject> _objects { get; private set; }

    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        ID = data.ID;
        transform.name = data.DefaultName + ID;
        _objects = new List<GameObject>();
    }

    public void AddObject(GameObject obj)
    {
        _objects.Add(obj);
    }

    public void RemoveObject(GameObject obj)
    {
        _objects.Remove(obj);
    }

    public void Highlight(Color highlightColor)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = highlightColor;
    }
}
