using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializable
{
    void OnInitialize(InitializationData data);
}

[System.Serializable]
public class InitializationData
{
    public Vector2Int GridPosition;
    public int ID;
    public string DefaultName;
    public Vector2Int InitialSpeed;


    public InitializationData(Vector2Int gridPosition, int id = 0, string name = null, int speedX = 0, int speedY = 0)
    {
        GridPosition = gridPosition;
        ID = id;
        DefaultName = name;
        InitialSpeed = new Vector2Int(speedX,speedY);
    }
}